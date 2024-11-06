
using Application;
using Application.Interfaces;
using Application.ServiceReponse;
using Application.Services;
using Application.Utils;
using Application.ViewModels.LiveBiddingDTOs;
using Castle.Core.Resource;
using Domain.Entity;
using Domain.Enums;
using Infrastructures;
using iTextSharp.text.pdf.parser.clipper;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using System.Security.Claims;
using WebAPI.Middlewares;

namespace WebAPI.Service
{
    public class LiveBiddingService
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly IHubContext<BiddingHub> _hubContext;

        public LiveBiddingService(IHubContext<BiddingHub> hubContext, IServiceProvider serviceProvider)
        {
            _hubContext = hubContext;
            _serviceProvider = serviceProvider;
        }
        public async Task ChecKLotEndAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();   
                var lotLiveBidding = _cacheService.GetHashLots(l => l.LotType == EnumLotType.Public_Auction.ToString() && l.Status == EnumStatusLot.Auctioning.ToString());              
                foreach (var lot in lotLiveBidding)
                {
                    var endTime = lot.EndTime;
                    if (endTime.HasValue && DateTime.UtcNow > endTime.Value)
                    {
                        await EndLot(lot.Id, endTime.Value);
                    }                   
                }               
            }
        }


        public async Task ChecKAuctionEndAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                var auctionLiveBidding = _unitOfWork.AuctionRepository.GetAuctionsAsync(EnumStatusAuction.Live.ToString());
                foreach (var auction in auctionLiveBidding)
                {
                    var lotsInAuction = _cacheService.GetHashLots(l => l.AuctionId == auction.Id);

                    
                    bool allLotsEnded = lotsInAuction.All(lot =>
                        lot.Status == EnumStatusLot.Sold.ToString() ||
                        lot.Status == EnumStatusLot.Passed.ToString() ||
                        lot.Status == EnumStatusLot.Canceled.ToString());

                   
                    var maxTimeAuction = _cacheService.GetHashLots(x => x.AuctionId == auction.Id).OrderByDescending(x => x.EndTime).FirstOrDefault();
                    if (allLotsEnded)
                    {
                        auction.Status = EnumStatusAuction.Past.ToString();
                        auction.ActualEndTime = maxTimeAuction.EndTime;
                        _unitOfWork.AuctionRepository.Update(auction);
                        await _unitOfWork.SaveChangeAsync();
                    }
                }
            }
        }



        public async Task ChecKLotEndReducedBiddingAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
               
                var lotLiveBidding = _unitOfWork.LotRepository.GetLotsAsync(EnumLotType.Auction_Price_GraduallyReduced.ToString(), EnumStatusLot.Auctioning.ToString());

             
                if (lotLiveBidding != null)
                {
                    foreach (var lot in lotLiveBidding)
                    {                       
                        var endTime = lot.EndTime;
                        if (endTime.HasValue && DateTime.UtcNow > endTime.Value)
                        {                          
                          await  EndTimeReducedBidding(lot.Id);
                        }
                        else
                        {
                            await ReduceTimeBidding(lot.Id);
                        }
                      
                    }
                }            
            }
        }

        public async Task CheckLotStartAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _claimsService = scope.ServiceProvider.GetRequiredService<IClaimsService>();
                var _lotService = scope.ServiceProvider.GetRequiredService<ILotService>();
                var auctionWithUpcoming = _unitOfWork.AuctionRepository.GetAuctionsAsync( EnumStatusAuction.UpComing.ToString());
                if (auctionWithUpcoming != null)
                {
                    foreach(var auction in auctionWithUpcoming)
                    {
                        var startTime = auction.StartTime;
                        if (startTime.HasValue && DateTime.UtcNow >= startTime.Value)
                        {
                            auction.Status = EnumStatusAuction.Live.ToString();
                            auction.ModificationDate = DateTime.Now;
                            auction.ModificationBy = _claimsService.GetCurrentUserId;

                            //lưu vào supabase
                            _unitOfWork.AuctionRepository.Update(auction);
                            await _unitOfWork.SaveChangeAsync();
                            //lưu cả lên redis ròi và supabase
                            await _lotService.UpdateLotRange(auction.Id, EnumStatusLot.Auctioning.ToString());
                            
                        }                          
                    }
                }               
            }
        }


        //check tu dong : khi khong co ai đấu giá và giá hiện tại vẫn cao hơn giá min và chưa đến giờ end lot thì sẽ tự động giảm giá xuống.
        public async Task EndTimeReducedBidding(int lotId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                string lotGroupName = $"lot-{lotId}";
                var bidPrices = _cacheService.GetSortedSetDataFilter<BidPrice>("BidPrice", l => l.LotId == lotId);
                var lot = _cacheService.GetLotById(lotId) ;
                if(bidPrices.Count ==  0)
                {
                    //cap nhat trang thai lot sold
                    lot.ActualEndTime = lot.EndTime;
                    lot.CurrentPrice = lot.CurrentPrice;
                    lot.Status = EnumStatusLot.Passed.ToString();
                    _unitOfWork.LotRepository.Update(lot);
                    _cacheService.UpdateLotStatus(lotId, lot.Status);
                    await _unitOfWork.SaveChangeAsync();
                    await _hubContext.Clients.Group(lotGroupName).SendAsync("AuctionEndedWithWinner", "Phiên đã kết thúc va khong co dau gia");

                }
            }          
        }

        public async Task ReduceTimeBidding(int lotId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                string lotGroupName = $"lot-{lotId}";

                var lot = _cacheService.GetLotById(lotId);
                
                //neu  den gio thi check xem co bidprice khong, neu cos  ket thuc phien va random winner
                // neu k co bidPrice thi tiep tuc giam, cap nhat lai actualEndTime cho den khi 
                


                var bidPrices = _cacheService.GetSortedSetDataFilter<BidPrice>("BidPrice", l => l.LotId == lotId);
                var currentPrice = lot.CurrentPrice ?? lot.StartPrice;

                await Task.Delay(10000);

                while (currentPrice > lot.FinalPriceSold && lot.Status == EnumStatusLot.Auctioning.ToString() && lot.EndTime > DateTime.UtcNow)
                {
                    
                        if (bidPrices.Count > 0)
                        {
                            //cap nhat trang thai lot sold
                            lot.ActualEndTime = DateTime.UtcNow;
                            lot.CurrentPrice = currentPrice;
                            lot.Status = EnumStatusLot.Sold.ToString();
                            _unitOfWork.LotRepository.Update(lot);
                            _cacheService.UpdateLotStatus(lotId, lot.Status);
  

                            //thuc hien random va xu ly cho nguoi chien thang, nguoi thua
                            Random random = new Random();
                            int winnerIndex = random.Next(bidPrices.Count);
                            var winnerBid = bidPrices[winnerIndex];

                            await HandleWinnerAndLoserLot(lotId, winnerBid);
                            break;
                        }
                        else
                        {

                            //neu k cos bid price thi ha gia va tang actualEndTime len theo bidIncrementTime,
                            //sau do doi BidIncrement Time roi lai tiep tuc kiem tra khi den gio endTime thi co bidPrice nao k
                            currentPrice = currentPrice - lot.BidIncrement;
                            lot.CurrentPrice = currentPrice;
                            if (currentPrice < lot.FinalPriceSold)
                            {
                                currentPrice = lot.FinalPriceSold;
                                lot.CurrentPrice = lot.FinalPriceSold;

                            }

                            _cacheService.UpdateLotCurrentPriceForReduceBidding(lotId, currentPrice);
                            await _hubContext.Clients.Group(lotGroupName).SendAsync("ReducePriceBidding", "Giá đã giảm!", currentPrice, DateTime.UtcNow);

                            var lotsql = await _unitOfWork.LotRepository.GetByIdAsync(lotId);
                            lotsql.CurrentPrice = currentPrice;
                            _unitOfWork.LotRepository.Update(lotsql);

                            await _unitOfWork.SaveChangeAsync();
                            
                            await Task.Delay(10000);
                           
                        }
                    //lay lai lot tren redis
                        lot = _cacheService.GetLotById(lotId);
                        bidPrices = _cacheService.GetSortedSetDataFilter<BidPrice>("BidPrice", l => l.LotId == lotId);
                    }
            }
        }


        private async Task HandleWinnerAndLoserLot(int lotId, BidPrice winnerBid)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                var bidPrices = _cacheService.GetSortedSetDataFilter<BidPrice>("BidPrice", l => l.LotId == lotId);


                //update customerLot
                var winnerCustomerLot = await _unitOfWork.CustomerLotRepository.GetCustomerLotByCustomerAndLot(winnerBid.CustomerId, lotId);
                winnerCustomerLot.IsWinner = true;
                winnerCustomerLot.CurrentPrice = winnerBid.CurrentPrice;
                string lotGroupName = $"lot-{lotId}";
                _unitOfWork.CustomerLotRepository.Update(winnerCustomerLot);


                await _hubContext.Clients.Group(lotGroupName).SendAsync("AuctionEndedWithWinner", "Phiên đã kết thúc!", winnerBid.CustomerId, winnerBid.CurrentPrice);
                //tao invoice cho wwinner
                var invoice = new Invoice
                {
                    CustomerId = winnerBid.CustomerId,
                    CustomerLotId = winnerCustomerLot.Id,
                    StaffId = winnerCustomerLot.Lot.StaffId,
                    Price = winnerBid.CurrentPrice,
                    Free = (float?)(winnerBid.CurrentPrice * 0.25),
                    TotalPrice = (float?)(winnerBid.CurrentPrice + winnerBid.CurrentPrice * 0.25 - winnerCustomerLot.Lot.Deposit),
                    CreationDate = DateTime.Now,
                    Status = EnumCustomerLot.CreateInvoice.ToString()
                };

                await _unitOfWork.InvoiceRepository.AddAsync(invoice);

                winnerCustomerLot.Status = EnumCustomerLot.CreateInvoice.ToString();
                winnerCustomerLot.IsInvoiced = true;
                _unitOfWork.CustomerLotRepository.Update(winnerCustomerLot);


                var historyCustomerlot = new HistoryStatusCustomerLot()
                {
                    Status = winnerCustomerLot.Status,
                    CustomerLotId = winnerCustomerLot.Id,
                    CurrentTime = DateTime.Now,
                };
                await _unitOfWork.HistoryStatusCustomerLotRepository.AddAsync(historyCustomerlot);
                await _unitOfWork.SaveChangeAsync();

                //lấy ra những thằng thua theo lot(group by theo customerId và lotId,Lấy giá trị đầu tiên của mỗi nhóm (distinct)
                var custemerLotGroupBy = bidPrices.GroupBy(b => new { b.CustomerId, b.LotId })
                                      .Select(g => g.First())
                                      .ToList();

                //lay ra lisst customerLot theo list customerid vaf lotId
                var losers = _unitOfWork.CustomerLotRepository.GetListCustomerLotByCustomerAndLot(custemerLotGroupBy, winnerCustomerLot.Id);
                if (losers != null)
                {
                    List<CustomerLot> listCustomerLot = new List<CustomerLot>();
                    foreach (var loser in losers)
                    {
                        loser.IsWinner = false;

                        //hoan coc cho loser
                        var walletOfLoser = await _unitOfWork.WalletRepository.GetByCustomerId(loser.CustomerId);
                        walletOfLoser.Balance = walletOfLoser.Balance + (decimal?)loser.Lot.Deposit;
                        _unitOfWork.WalletRepository.Update(walletOfLoser);
                        loser.IsRefunded = true;
                        loser.Status = EnumCustomerLot.Refunded.ToString();

                        listCustomerLot.Add(loser);
                        //lưu history của loser là refunded
                        var historyCustomerlotLoser = new HistoryStatusCustomerLot()
                        {
                            Status = loser.Status,
                            CustomerLotId = loser.Id,
                            CurrentTime = DateTime.Now,
                        };
                        await _unitOfWork.HistoryStatusCustomerLotRepository.AddAsync(historyCustomerlotLoser);


                        //cap nhat transaction vi
                        var walletTrasaction = new WalletTransaction
                        {
                            transactionType = EnumTransactionType.RefundDeposit.ToString(),
                            DocNo = loser.Id,
                            Amount = winnerCustomerLot.Lot.Deposit,
                            TransactionTime = DateTime.UtcNow,
                            Status = "Completed"
                        };
                        await _unitOfWork.WalletTransactionRepository.AddAsync(walletTrasaction);


                        //cap nhat transaction cty
                        var trasaction = new Transaction
                        {
                            TransactionType = EnumTransactionType.RefundDeposit.ToString(),
                            DocNo = loser.Id,
                            Amount = winnerCustomerLot.Lot.Deposit,
                            TransactionTime = DateTime.UtcNow,

                        };
                        await _unitOfWork.TransactionRepository.AddAsync(trasaction);
                        await _unitOfWork.SaveChangeAsync();
                    }
                    _unitOfWork.CustomerLotRepository.UpdateRange(listCustomerLot);
                }
                await _unitOfWork.SaveChangeAsync();
            }
        }

        private async Task StartLot(int lotId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                var lot = await _unitOfWork.LotRepository.GetByIdAsync(lotId);
                if (lot == null)
                {
                    throw new Exception("Not found Lot Id");
                }
                lot.Status = EnumStatusLot.Auctioning.ToString();
                _unitOfWork.LotRepository.Update(lot);
                await _unitOfWork.SaveChangeAsync();

                _cacheService.UpdateLotStatus(lotId, lot.Status);
            }

        }

        private async Task EndLot(int lotId, DateTime endTime)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                string lotGroupName = $"lot-{lotId}";
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                var lot = await _unitOfWork.LotRepository.GetByIdAsync(lotId);
                if (lot == null)
                {
                    throw new Exception("Not found Lot Id");
                }
                else
                {
                    // Truy xuất dữ liệu bid prices từ redis 
                    var bidPrices = _cacheService.GetSortedSetDataFilter<BidPrice>("BidPrice", l => l.LotId == lot.Id);
                    await _unitOfWork.BidPriceRepository.AddRangeAsync(bidPrices);

                    // nếu lot đó k có ai đấu giá thì đổi qua status passed, cập nhật lên cả redis và sql
                    if (bidPrices.Count == 0)
                    {

                        lot.Status = EnumStatusLot.Passed.ToString();
                        lot.ActualEndTime = endTime;

                        _unitOfWork.LotRepository.Update(lot);

                        _cacheService.UpdateLotStatus(lotId, lot.Status);
                        _cacheService.UpdateLotActualEndTime(lotId, endTime);
                        await _unitOfWork.SaveChangeAsync();
                        await _hubContext.Clients.Group(lotGroupName).SendAsync("AuctionEnded", "Phiên đã kết thúc va khong co ai dau gia!");

                    }
                    else
                    {
                        //ngược lại, nếu có bid prices thì truy xuất ra gía max nhất( lưu theo sortedSet và có sắp xếp giảm dần ở hàm get
                        //nên chỉ cần lấy thằng đầu tiên)
                        //lưu status lot là sold
                        //  cập nhật lên cả redis và sql; cập nhật endLot actual, giá bán được
                        var winner = bidPrices.FirstOrDefault();
                        lot.Status = EnumStatusLot.Sold.ToString();
                        lot.ActualEndTime = endTime;
                        lot.CurrentPrice = winner.CurrentPrice;
                        _unitOfWork.LotRepository.Update(lot);
                        _cacheService.UpdateLotStatus(lotId, lot.Status);
                        await _unitOfWork.SaveChangeAsync();

                        await _hubContext.Clients.Group(lotGroupName).SendAsync("AuctionEndedWithWinner", "Phiên đã kết thúc!", winner.CustomerId, winner.CurrentPrice);

                        //xu ly cho thang thắng va thua                       
                        await   HandleWinnerAndLoserLot(lot.Id, winner);
                    }
                }
               
            }
        }

        private async Task<BidPrice> GetWinnerInEndLot(Lot lot)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var maxPriceInLot = lot.BidPrices.Max(x => x.CurrentPrice);
                var maxBidPriceInLots = lot.BidPrices.Where(x => (lot.LotType == EnumLotType.Fixed_Price.ToString())? x.CurrentPrice == lot.BuyNowPrice : x.CurrentPrice == maxPriceInLot).ToList();
                if (maxBidPriceInLots.Count > 1)
                {
                    Random random = new Random();
                    int randomIndex = random.Next(maxBidPriceInLots.Count);
                    BidPrice randomBidPriceWinner = maxBidPriceInLots[randomIndex];
                    return randomBidPriceWinner;
                }
                else
                {
                    return null;
                }

            }
        }
        private async Task<BidPrice> GetWinnerBuyNowPrice(Lot lot)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var bidPriceWin = lot.BidPrices.FirstOrDefault(x => (lot.LotType == EnumLotType.Fixed_Price.ToString())? x.CurrentPrice == lot.BuyNowPrice : x.CurrentPrice == lot.FinalPriceSold);
                if (bidPriceWin != null)
                {
                    return bidPriceWin;
                }
            }
            return null;
        }
        public async Task CheckLotFixedPriceAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _foorFeeService = scope.ServiceProvider.GetRequiredService<IFoorFeePercentService>();
                //Xu ly luc tg ket thuc
                var lotEnds = await _unitOfWork.LotRepository.GetAllAsync(x => x.EndTime <= DateTime.UtcNow 
                                                                            && x.LotType == EnumLotType.Fixed_Price.ToString() 
                                                                            && x.Status.ToLower().Trim().Equals(EnumStatusLot.Auctioning.ToString().ToLower().Trim()));
                Invoice invoice;
                if (lotEnds.Count > 0)
                {
                    foreach(var lot in lotEnds)
                    {
                        var bidPriceWinner = await GetWinnerInEndLot(lot);
                        if (bidPriceWinner != null)
                        {
                            lot.CurrentPrice = bidPriceWinner.CurrentPrice;
                            lot.CustomerLots.First(x => x.CustomerId == bidPriceWinner?.CustomerId).CurrentPrice = bidPriceWinner.CurrentPrice;
                            lot.CustomerLots.First(x => x.CustomerId == bidPriceWinner?.CustomerId).IsWinner = true;
                            var fee = bidPriceWinner.CurrentPrice * await _foorFeeService.GetPercentFloorFeeOfLot((float)bidPriceWinner.CurrentPrice);
                            invoice = new Invoice
                            {
                                CustomerId = bidPriceWinner.CustomerId,
                                CustomerLotId = lot.CustomerLots.First(x => x.CustomerId == bidPriceWinner?.CustomerId).Id,
                                StaffId = lot.StaffId,
                                Price = bidPriceWinner.CurrentPrice,
                                Free = fee,
                                TotalPrice = bidPriceWinner.CurrentPrice + fee - lot.Deposit,
                                CreationDate = DateTime.Now,
                                Status = EnumCustomerLot.CreateInvoice.ToString()
                            };
                            await _unitOfWork.InvoiceRepository.AddAsync(invoice);
                        }

                        lot.ActualEndTime = DateTime.UtcNow;
                        lot.Status = EnumStatusLot.Passed.ToString();
                        string lotGroupName = $"lot-{lot.Id}";
                        await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPriceforFixedPriceBiddingAuto", "Phiên đã kết thúc!");
                        await _unitOfWork.SaveChangeAsync();
                    }
                }


            }
        }
        public async Task CheckLotSercetAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _foorFeeService = scope.ServiceProvider.GetRequiredService<IFoorFeePercentService>();
                var lotEnds = await _unitOfWork.LotRepository.GetAllAsync(x => x.EndTime <= DateTime.UtcNow && x.LotType == EnumLotType.Secret_Auction.ToString() && x.Status.ToLower().Trim().Equals(EnumStatusLot.Auctioning.ToString().ToLower().Trim()));
                Invoice invoice;
                if (lotEnds.Any())
                {
                    foreach (var lot in lotEnds)
                    {
                        var bidPriceWinner = await GetWinnerInEndLot(lot);
                        if(bidPriceWinner != null)
                        {
                            lot.CurrentPrice = bidPriceWinner.CurrentPrice;
                            lot.CustomerLots.First(x => x.CustomerId == bidPriceWinner?.CustomerId).CurrentPrice = bidPriceWinner.CurrentPrice;
                            lot.CustomerLots.First(x => x.CustomerId == bidPriceWinner?.CustomerId).IsWinner = true;
                            var fee = bidPriceWinner.CurrentPrice *  await _foorFeeService.GetPercentFloorFeeOfLot((float)bidPriceWinner.CurrentPrice);
                            invoice = new Invoice
                            {
                                CustomerId = bidPriceWinner.CustomerId,
                                CustomerLotId = lot.CustomerLots.First(x => x.CustomerId == bidPriceWinner.CustomerId).Id,
                                StaffId = lot.StaffId,
                                Price = bidPriceWinner.CurrentPrice,
                                Free = fee,
                                TotalPrice = bidPriceWinner.CurrentPrice + fee - lot.Deposit,
                                CreationDate = DateTime.Now,
                                Status = EnumCustomerLot.CreateInvoice.ToString()
                            };
                            await _unitOfWork.InvoiceRepository.AddAsync(invoice);
                        }
                        lot.Status = EnumStatusLot.Passed.ToString();
                        string lotGroupName = $"lot-{lot.Id}";
                        await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPriceforSercetBiddingAuto", "Phiên đã kết thúc!");
                        lot.ActualEndTime = DateTime.UtcNow;
                        await _unitOfWork.SaveChangeAsync();
                    }
                }
            }
        }
        public async Task AutoBidAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _customerLotService = scope.ServiceProvider.GetRequiredService<ICustomerLotService>();
                var customerLots = await _unitOfWork.CustomerLotRepository.GetAllAsync(x => x.AutoBids.FirstOrDefault().IsActive == true);
                try
                {
                    foreach (var player in customerLots) 
                    {   
                        if (await _customerLotService.CheckTimeAutoBid(player.Id))
                        {
                            var bidPriceFuture = player.CurrentPrice + player.AutoBids.FirstOrDefault(x => x.IsActive == true).NumberOfPriceStep;
                            var (isFuturePrice, price) = await _customerLotService.CheckBidPriceTop((float)bidPriceFuture, player.AutoBids.FirstOrDefault(x => x.IsActive == true));
                            if (isFuturePrice == true)
                            {
                                await _customerLotService.UpdateAutoBidPrice(player.Id, (float)price);
                                string lotGroupName = $"lot-{player.LotId}";
                                await _hubContext.Clients.Group(lotGroupName).SendAsync("AutoBid", "AutoBid End Time");
                            }
                        }
                    }
                }catch(Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }
    }
}

