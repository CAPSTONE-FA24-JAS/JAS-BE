
using Application;
using Application.Interfaces;
using Application.ViewModels.CustomerLotDTOs;
using Domain.Entity;
using Domain.Enums;
using iTextSharp.text.pdf.parser.clipper;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using WebAPI.Middlewares;

namespace WebAPI.Service
{
    public class LiveBiddingService
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly IHubContext<BiddingHub> _hubContext;
        private readonly IHubContext<NotificationHub> _notificationHub;

        public LiveBiddingService(IHubContext<BiddingHub> hubContext, IServiceProvider serviceProvider, IHubContext<NotificationHub> notificationHub)
        {
            _hubContext = hubContext;
            _serviceProvider = serviceProvider;
            _notificationHub = notificationHub;
        }
        public async Task ChecKLotEndAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                var lotLiveBidding = await _unitOfWork.LotRepository.GetAllAsync(x => x.LotType == EnumLotType.Public_Auction.ToString() && (x.Status == EnumStatusLot.Auctioning.ToString() ||
                                                                                           x.Status == EnumStatusLot.Pause.ToString()));
                foreach (var lotsql in lotLiveBidding)
                {
                    var lot = _cacheService.GetLotById(lotsql.Id);
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
                    var lotsInAuction = await _unitOfWork.LotRepository.GetAllAsync(x => x.AuctionId == auction.Id);


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
                        await _hubContext.Clients.All.SendAsync("AcutionHasBeenEnd", "Phiên đã được dong", auction.Id);
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

                var lotLiveBidding = await _unitOfWork.LotRepository.GetAllAsync(x => x.LotType == EnumLotType.Auction_Price_GraduallyReduced.ToString() && (x.Status == EnumStatusLot.Auctioning.ToString() ||
                                                                                           x.Status == EnumStatusLot.Pause.ToString()));


                if (lotLiveBidding != null)
                {
                    foreach (var lot in lotLiveBidding)
                    {
                        var endTime = lot.EndTime;
                        if (endTime.HasValue && DateTime.UtcNow > endTime.Value)
                        {
                            await EndTimeReducedBidding(lot.Id);
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
                var auctionWithUpcoming = _unitOfWork.AuctionRepository.GetAuctionsAsync(EnumStatusAuction.UpComing.ToString());
                if (auctionWithUpcoming != null)
                {
                    foreach (var auction in auctionWithUpcoming)
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

                            await _notificationHub.Clients.All.SendAsync("AcutionHasBeenStarted", "Phiên đã được mở", auction.Id);
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
                string redisKey = $"BidPrice:{lotId}";
                var bidPrices = _cacheService.GetSortedSetDataFilter<BidPrice>(redisKey, l => l.LotId == lotId);
                var lot = _cacheService.GetLotById(lotId);
                var lotsql = await _unitOfWork.LotRepository.GetByIdAsync(lotId);
                if(lot.Status == EnumStatusLot.Auctioning.ToString())
                {
                    if (bidPrices.Count == 0)
                    {
                        _cacheService.UpdateLotStatus(lotId, EnumStatusLot.Passed.ToString());
                        

                        lotsql.ActualEndTime = lot.EndTime;
                        lotsql.CurrentPrice = lot.CurrentPrice;
                        lotsql.Status = EnumStatusLot.Passed.ToString();
                        _unitOfWork.LotRepository.Update(lotsql);
                        await _unitOfWork.SaveChangeAsync();
                        await HandleLoserLot(lotId);
                        await _hubContext.Clients.Group(lotGroupName).SendAsync("AuctionEndedWithWinnerReduce", "Phiên đã kết thúc va khong co dau gia");

                    }
                    else
                    {
                        if (lot.Status == EnumStatusLot.Auctioning.ToString())
                        {
                            Random random = new Random();
                            int winnerIndex = random.Next(bidPrices.Count);
                            var winnerBid = bidPrices[winnerIndex];

                            //cap nhat trang thai lot sold
                            lot.CurrentPrice = winnerBid.CurrentPrice;
                            lot.ActualEndTime = DateTime.UtcNow;
                            lot.Status = EnumStatusLot.Sold.ToString();
                            _cacheService.UpdateLotStatus(lotId, EnumStatusLot.Sold.ToString());
                            lot = _cacheService.GetLotById(lotId);


                            lotsql.CurrentPrice = winnerBid.CurrentPrice;
                            
                            lotsql.ActualEndTime = lot.EndTime;
                            lotsql.Status = EnumStatusLot.Sold.ToString();
                            _unitOfWork.LotRepository.Update(lotsql);
                            await _unitOfWork.SaveChangeAsync();
                            await _hubContext.Clients.Group(lotGroupName).SendAsync("AuctionEndedReduceBidding", "Phien đa ket thuc", winnerBid.CustomerId, winnerBid.CurrentPrice);
                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeForReduceBidding", "Thoi gian ket actual", lot.ActualEndTime);
                            //xu ly cho thang thang va thua
                            await HandleWinnerAndLoserLot(lotId, winnerBid);
                        }
                    }
                }
                else
                {
                    
                  //  await _unitOfWork.BidPriceRepository.AddRangeAsync(bidPrices);
                    await _hubContext.Clients.Group(lotGroupName).SendAsync("AuctionPublicEnded", "Phiên đã kết thúc!");
                    lotsql.Status = EnumStatusLot.Passed.ToString();
                    lotsql.ActualEndTime = lotsql.EndTime;

                    _unitOfWork.LotRepository.Update(lotsql);

                    _cacheService.UpdateLotStatus(lotId, EnumStatusLot.Passed.ToString());
                    _cacheService.UpdateLotActualEndTime(lotId, (DateTime)lotsql.EndTime);
                    await _unitOfWork.SaveChangeAsync();

                    await HandleLoserLot(lotId);
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

                if(lot.Status == EnumStatusLot.Auctioning.ToString())
                {
                    string redisKey = $"BidPrice:{lotId}";
                    var bidPrices = _cacheService.GetSortedSetDataFilter<BidPrice>(redisKey, l => l.LotId == lotId);


                    var currentPrice = lot.CurrentPrice ?? lot.StartPrice;
                    _cacheService.UpdateLotCurrentPriceForReduceBidding(lotId, currentPrice);
                    await _hubContext.Clients.Group(lotGroupName).SendAsync("CurrentPriceForReduceBiddingWhenStartLot", "Giá hien tai!", currentPrice, DateTime.UtcNow);


                    await Task.Delay(TimeSpan.FromSeconds((int)lot.BidIncrementTime));

                    while (currentPrice > lot.FinalPriceSold && lot.Status == EnumStatusLot.Auctioning.ToString() && lot.EndTime > DateTime.UtcNow)
                    {

                        if (bidPrices.Count > 0)
                        {

                            //thuc hien random va xu ly cho nguoi chien thang, nguoi thua
                            Random random = new Random();
                            int winnerIndex = random.Next(bidPrices.Count);
                            var winnerBid = bidPrices[winnerIndex];

                            //cap nhat trang thai lot sold
                            _cacheService.UpdateLotCurrentPriceForReduceBidding(lotId, winnerBid.CurrentPrice);
                            _cacheService.UpdateLotStatus(lotId, EnumStatusLot.Sold.ToString());
                            _cacheService.UpdateLotActualEndTime(lotId, DateTime.UtcNow);
                            lot = _cacheService.GetLotById(lotId);
                            var lotsql = await _unitOfWork.LotRepository.GetByIdAsync(lotId);

                            lotsql.CurrentPrice = winnerBid.CurrentPrice;
                           
                            lotsql.ActualEndTime = DateTime.UtcNow;
                            lotsql.Status = EnumStatusLot.Sold.ToString();
                            _unitOfWork.LotRepository.Update(lotsql);
                           // await _unitOfWork.BidPriceRepository.AddRangeAsync(bidPrices);
                            await _unitOfWork.SaveChangeAsync();
                            await _hubContext.Clients.Group(lotGroupName).SendAsync("AuctionEndedReduceBidding", "Phien đa ket thuc", winnerBid.CustomerId, winnerBid.CurrentPrice);
                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeForReduceBidding", "Thoi gian ket actual", lot.ActualEndTime);
                            //xu ly cho thang thang va thua
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

                            await Task.Delay(TimeSpan.FromSeconds((int)lot.BidIncrementTime));

                        }
                        //lay lai lot tren redis
                        lot = _cacheService.GetLotById(lotId);
                        bidPrices = _cacheService.GetSortedSetDataFilter<BidPrice>(redisKey, l => l.LotId == lotId);
                    }                 
                }
                
            }
        }


        private async Task HandleWinnerAndLoserLot(int lotId, BidPrice winnerBid)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _floorFeeService = scope.ServiceProvider.GetRequiredService<IFoorFeePercentService>();
                var _cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                string redisKey = $"BidPrice:{lotId}";
                var bidPrices = _cacheService.GetSortedSetDataFilter<BidPrice>(redisKey, l => l.LotId == lotId);
                var lot = await _unitOfWork.LotRepository.GetByIdAsync(lotId);

                //update customerLot
                var winnerCustomerLot = await _unitOfWork.CustomerLotRepository.GetCustomerLotByCustomerAndLot(winnerBid.CustomerId, lotId);
                winnerCustomerLot.IsWinner = true;
                winnerCustomerLot.CurrentPrice = winnerBid.CurrentPrice;
                string lotGroupName = $"lot-{lotId}";
                _unitOfWork.CustomerLotRepository.Update(winnerCustomerLot);



                //tao invoice cho wwinner
                var invoice = new Invoice
                {
                    CustomerId = winnerBid.CustomerId,
                    CustomerLotId = winnerCustomerLot.Id,
                    StaffId = winnerCustomerLot.Lot.StaffId,
                    Price = winnerBid.CurrentPrice,
                    Free = (winnerBid.CurrentPrice * await _floorFeeService.GetPercentFloorFeeOfLot((float)winnerBid.CurrentPrice)),
                    TotalPrice = (winnerBid.CurrentPrice + (winnerBid.CurrentPrice * await _floorFeeService.GetPercentFloorFeeOfLot((float)winnerBid.CurrentPrice)) - winnerCustomerLot.Lot.Deposit),
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
                    CurrentTime = DateTime.UtcNow,
                };
                await _unitOfWork.HistoryStatusCustomerLotRepository.AddAsync(historyCustomerlot);
                await _unitOfWork.SaveChangeAsync();

                //tao notification
                var notification = new Notification
                {
                    Title = $"Bidding win in Lot {lotId}",
                    Description = $" You won in lot {lotId} and system auto created invoice for you.",
                    Is_Read = false,
                    NotifiableId = invoice.Id,
                    AccountId = winnerCustomerLot.Customer.AccountId,
                    CreationDate = DateTime.UtcNow,
                    Notifi_Type = "CreateInvoice",
                    ImageLink = lot.Jewelry.ImageJewelries.FirstOrDefault()?.ImageLink

                };

                await _unitOfWork.NotificationRepository.AddAsync(notification);
                await _unitOfWork.SaveChangeAsync();
                await _notificationHub.Clients.Group(winnerCustomerLot.Customer.AccountId.ToString()).SendAsync("NewNotificationReceived", "Có thông báo mới!");


                //lay ra list customerLot theo  lotId trừ thằng thắng
                var losers = _unitOfWork.CustomerLotRepository.GetListCustomerLotByCustomerAndLot(lotId, winnerCustomerLot.Id);
                if (losers != null)
                {
                    List<CustomerLot> listCustomerLot = new List<CustomerLot>();
                    foreach (var loser in losers)
                    {
                        loser.IsWinner = false;

                        //hoan coc cho loser
                        var walletOfLoser = await _unitOfWork.WalletRepository.GetByCustomerId(loser.CustomerId);
                        walletOfLoser.AvailableBalance += (decimal?)loser?.Lot?.Deposit ?? 0;
                        walletOfLoser.Balance = walletOfLoser.AvailableBalance ?? 0 + walletOfLoser.FrozenBalance ?? 0;
                        _unitOfWork.WalletRepository.Update(walletOfLoser);
                        loser.IsRefunded = true;
                        loser.Status = EnumCustomerLot.Refunded.ToString();
                        listCustomerLot.Add(loser);

                        var maxBidPriceLoser = await _unitOfWork.BidPriceRepository.GetMaxBidPriceByCustomerIdAndLot(loser.CustomerId, lotId);
                        if (maxBidPriceLoser == null)
                        {
                            loser.CurrentPrice = 0;
                        }
                        else
                        {
                            loser.CurrentPrice = maxBidPriceLoser.CurrentPrice;
                        }

                        _unitOfWork.CustomerLotRepository.Update(loser);
                        //lưu history của loser là refunded
                        var historyCustomerlotLoser = new HistoryStatusCustomerLot()
                        {
                            Status = loser.Status,
                            CustomerLotId = loser.Id,
                            CurrentTime = DateTime.Now,
                        };
                        await _unitOfWork.HistoryStatusCustomerLotRepository.AddAsync(historyCustomerlotLoser);

                        //tao notification cho loser
                        var notificationloser = new Notification
                        {
                            Title = $"Bidding lose in lot {lotId}",
                            Description = $" You had been lose in lot {lotId} và system auto refunded deposit for you",
                            Is_Read = false,
                            NotifiableId = loser.Id,  //cusrtomerLot => dẫn tới myBid
                            AccountId = loser.Customer.AccountId,
                            CreationDate = DateTime.UtcNow,
                            Notifi_Type = "Refunded",
                            ImageLink = lot.Jewelry.ImageJewelries.FirstOrDefault()?.ImageLink

                            //  ImageLink = lot.Jewelry.ImageJewelries.FirstOrDefault().ImageLink
                        };

                        await _unitOfWork.NotificationRepository.AddAsync(notificationloser);
                        await _notificationHub.Clients.Group(loser.Customer.AccountId.ToString()).SendAsync("NewNotificationReceived", "Có thông báo mới!");
                        //cap nhat transaction vi
                        var walletTrasaction = new WalletTransaction
                        {
                            transactionType = EnumTransactionType.RefundDeposit.ToString(),
                            DocNo = loser.Id,
                            Amount = winnerCustomerLot.Lot.Deposit,
                            TransactionTime = DateTime.UtcNow,
                            Status = "Completed",
                            WalletId = loser.Customer.Wallet.Id,
                            transactionPerson = loser.Customer.Id
                        };
                        await _unitOfWork.WalletTransactionRepository.AddAsync(walletTrasaction);


                        //cap nhat transaction cty
                        var trasaction = new Transaction
                        {
                            TransactionType = EnumTransactionType.RefundDeposit.ToString(),
                            DocNo = loser.Id,
                            Amount = winnerCustomerLot.Lot.Deposit,
                            TransactionTime = DateTime.UtcNow,
                            TransactionPerson = loser.CustomerId

                        };
                        await _unitOfWork.TransactionRepository.AddAsync(trasaction);

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
                    if (lot.Status == EnumStatusLot.Auctioning.ToString())
                    {
                        string redisKey = $"BidPrice:{lotId}";
                        // Truy xuất dữ liệu bid prices từ redis 
                        var bidPrices = _cacheService.GetSortedSetDataFilter<BidPrice>(redisKey, l => l.LotId == lot.Id);
                        await _unitOfWork.BidPriceRepository.AddRangeAsync(bidPrices);

                        // nếu lot đó k có ai đấu giá thì đổi qua status passed, cập nhật lên cả redis và sql
                        if (bidPrices.Count == 0)
                        {
                            await _hubContext.Clients.Group(lotGroupName).SendAsync("AuctionPublicEnded", "Phiên đã kết thúc va khong co ai dau gia!");
                            lot.Status = EnumStatusLot.Passed.ToString();
                            lot.ActualEndTime = endTime;

                            _unitOfWork.LotRepository.Update(lot);

                            _cacheService.UpdateLotStatus(lotId, lot.Status);
                            _cacheService.UpdateLotActualEndTime(lotId, endTime);
                            await _unitOfWork.SaveChangeAsync();


                        }
                        else
                        {
                            //ngược lại, nếu có bid prices thì truy xuất ra gía max nhất( lưu theo sortedSet và có sắp xếp giảm dần ở hàm get
                            //nên chỉ cần lấy thằng đầu tiên)
                            //lưu status lot là sold
                            //  cập nhật lên cả redis và sql; cập nhật endLot actual, giá bán được

                            await _hubContext.Clients.Group(lotGroupName).SendAsync("AuctionPublicEnded", "Phiên đã kết thúc!");
                            var winner = bidPrices.Where(x => x.Status == "Success").FirstOrDefault();
                            lot.Status = EnumStatusLot.Sold.ToString();
                            lot.ActualEndTime = endTime;
                            lot.CurrentPrice = winner.CurrentPrice;
                            
                            _unitOfWork.LotRepository.Update(lot);
                            _cacheService.UpdateLotStatus(lotId, lot.Status);
                            _cacheService.UpdateLotCurrentPriceForReduceBidding(lotId, winner.CurrentPrice);
                            await _unitOfWork.SaveChangeAsync();

                            await _hubContext.Clients.Group(lotGroupName).SendAsync("AuctionEndedWithWinnerPublic", "Phiên đã kết thúc!", winner.CustomerId, winner.CurrentPrice);

                            //xu ly cho thang thắng va thua                       
                            await HandleWinnerAndLoserLot(lot.Id, winner);
                        }
                    }
                    else
                    {
                        string redisKey = $"BidPrice:{lotId}";
                        // Truy xuất dữ liệu bid prices từ redis 
                        var bidPrices = _cacheService.GetSortedSetDataFilter<BidPrice>(redisKey, l => l.LotId == lot.Id);
                        await _unitOfWork.BidPriceRepository.AddRangeAsync(bidPrices);
                        await _hubContext.Clients.Group(lotGroupName).SendAsync("AuctionPublicEnded", "Phiên đã kết thúc!");
                        lot.Status = EnumStatusLot.Passed.ToString();
                        lot.ActualEndTime = endTime;

                        _unitOfWork.LotRepository.Update(lot);

                        _cacheService.UpdateLotStatus(lotId, lot.Status);
                        _cacheService.UpdateLotActualEndTime(lotId, endTime);
                        await _unitOfWork.SaveChangeAsync();

                        await HandleLoserLot(lotId);


                    }

                }

            }
        }

        private async Task<BidPrice> GetWinnerInEndLot(Lot lot)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var maxPriceInLot = lot.BidPrices.Max(x => x.CurrentPrice);
                var maxBidPriceInLots = lot.BidPrices.Where(x =>
                                            (lot.LotType == EnumLotType.Fixed_Price.ToString()) ? x.CurrentPrice == lot.BuyNowPrice : x.CurrentPrice == maxPriceInLot)
                                            .ToList();
                if (maxBidPriceInLots.Count > 1)
                {
                    Random random = new Random();
                    int randomIndex = random.Next(maxBidPriceInLots.Count);
                    BidPrice randomBidPriceWinner = maxBidPriceInLots[randomIndex];
                    return randomBidPriceWinner;
                }
                if (maxBidPriceInLots.Count > 0)
                {
                    return maxBidPriceInLots.FirstOrDefault();
                }
                return null;
            }
        }
        private async Task<BidPrice> GetWinnerBuyNowPrice(Lot lot)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var bidPriceWin = lot.BidPrices.FirstOrDefault(x => (lot.LotType == EnumLotType.Fixed_Price.ToString()) ? x.CurrentPrice == lot.BuyNowPrice : x.CurrentPrice == lot.FinalPriceSold);
                if (bidPriceWin != null)
                {
                    return bidPriceWin;
                }
            }
            return null;
        }

        private async Task SetLoser(List<CustomerLot> customerLot)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();
                await _walletService.RefundToWalletForUsersAsync(customerLot);
                foreach (var loser in customerLot)
                {
                    loser.IsWinner = false;
                    loser.IsRefunded = true;
                    loser.Status = EnumCustomerLot.Refunded.ToString();
                    var notification = new Notification
                    {
                        Title = $"Bidding lose in lot {loser.LotId}",
                        Description = $" You had been lose in lot {loser.LotId} và system auto refunded deposit for you",
                        Is_Read = false,
                        NotifiableId = loser.Id,
                        AccountId = loser.Customer.AccountId,
                        CreationDate = DateTime.UtcNow,
                        Notifi_Type = "Refunded",
                        ImageLink = loser.Lot.Jewelry.ImageJewelries.FirstOrDefault()?.ImageLink
                    };

                    await _unitOfWork.NotificationRepository.AddAsync(notification);
                    await _notificationHub.Clients.Group(loser.Customer.AccountId.ToString()).SendAsync("NewNotificationReceived", "Có thông báo mới!");
                }
                await _unitOfWork.SaveChangeAsync();
            }
        }

        public async Task CheckLotFixedPriceAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _foorFeeService = scope.ServiceProvider.GetRequiredService<IFoorFeePercentService>();
                var _customerLotService = scope.ServiceProvider.GetRequiredService<ICustomerLotService>();
                var _cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                var lotEnds = await _unitOfWork.LotRepository.GetAllAsync(x => x.EndTime <= DateTime.UtcNow
                                                                            && x.LotType == EnumLotType.Fixed_Price.ToString()
                                                                            && x.Status.ToLower().Trim().Equals(EnumStatusLot.Auctioning.ToString().ToLower().Trim()));
                var lotPauseEnds = await _unitOfWork.LotRepository.GetAllAsync(x => x.EndTime <= DateTime.UtcNow && x.LotType == EnumLotType.Fixed_Price.ToString() 
                                                                                    && x.Status.ToLower().Trim().Equals(EnumStatusLot.Pause.ToString().ToLower().Trim()));
                Invoice invoice;
                // thuc hienn lot th pause
                if (lotPauseEnds.Any())
                {
                    foreach (var lot in lotPauseEnds)
                    {
                        lot.Status = EnumStatusLot.Passed.ToString();
                        var losers = lot.CustomerLots?.ToList();
                        await SetLoser(losers);
                        _cacheService.UpdateLotStatus(lot.Id, lot.Status);
                        string lotGroupName = $"lot-{lot.Id}";
                        await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPriceforSercetBiddingAuto", "Phiên đã kết thúc!");
                        lot.ActualEndTime = DateTime.UtcNow;
                        await _unitOfWork.SaveChangeAsync();
                    }
                }
                // truong hop lot auctioning
                if (lotEnds.Count > 0)
                {
                    foreach (var lot in lotEnds)
                    {
                        var bidPriceWinner = await GetWinnerInEndLot(lot);
                        if (bidPriceWinner != null)
                        {
                            lot.CurrentPrice = bidPriceWinner.CurrentPrice;
                            lot.CustomerLots.First(x => x.CustomerId == bidPriceWinner?.CustomerId).CurrentPrice = bidPriceWinner.CurrentPrice;
                            lot.CustomerLots.First(x => x.CustomerId == bidPriceWinner?.CustomerId).IsWinner = true;
                            lot.CustomerLots.First(x => x.CustomerId == bidPriceWinner?.CustomerId).IsInvoiced = true;
                            lot.CustomerLots.First(x => x.CustomerId == bidPriceWinner?.CustomerId).Status = EnumCustomerLot.CreateInvoice.ToString();
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


                            var historyStatusCustomerLot = new HistoryStatusCustomerLot()
                            {
                                CustomerLotId = lot.CustomerLots.First(x => x.CustomerId == bidPriceWinner?.CustomerId).Id,
                                Status = EnumCustomerLot.CreateInvoice.ToString(),
                                CurrentTime = DateTime.UtcNow,
                            };
                            _customerLotService.CreateHistoryCustomerLot(historyStatusCustomerLot);
                            await _unitOfWork.InvoiceRepository.AddAsync(invoice);
                            lot.Status = EnumStatusLot.Sold.ToString();

                            //noti cho winner
                            var notification = new Notification
                            {
                                Title = $"Bidding win in Lot {lot.Id}",
                                Description = $" You won in lot {lot.Id} and system auto created invoice for you.",
                                Is_Read = false,
                                NotifiableId = invoice.Id,
                                AccountId = bidPriceWinner.Customer?.AccountId,
                                CreationDate = DateTime.UtcNow,
                                Notifi_Type = "CreateInvoice",
                                ImageLink = lot.Jewelry?.ImageJewelries?.FirstOrDefault()?.ImageLink

                            };
                            await _unitOfWork.NotificationRepository.AddAsync(notification);
                            await _unitOfWork.SaveChangeAsync();
                            await _notificationHub.Clients.Group(bidPriceWinner.Customer.AccountId.ToString()).SendAsync("NewNotificationReceived", "Có thông báo mới!");

                            var losers = lot.CustomerLots.Where(x => x.CustomerId != bidPriceWinner.CustomerId).ToList();
                            await SetLoser(losers);
                        }
                        else
                        {
                            lot.Status = EnumStatusLot.Passed.ToString();
                            var losers = lot.CustomerLots?
                                         .Where(x => x.CustomerId != (bidPriceWinner?.CustomerId ?? -1))
                                         .ToList();

                            await SetLoser(losers);
                        }

                        _cacheService.UpdateLotStatus(lot.Id, lot.Status);
                        lot.ActualEndTime = DateTime.UtcNow;
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
                var _cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                var _customerLotService = scope.ServiceProvider.GetRequiredService<ICustomerLotService>();
                var lotEnds = await _unitOfWork.LotRepository.GetAllAsync(x => x.EndTime <= DateTime.UtcNow && x.LotType == EnumLotType.Secret_Auction.ToString() && x.Status.ToLower().Trim().Equals(EnumStatusLot.Auctioning.ToString().ToLower().Trim()));
                var lotPauseEnds = await _unitOfWork.LotRepository.GetAllAsync(x => x.EndTime <= DateTime.UtcNow && x.LotType == EnumLotType.Secret_Auction.ToString() && x.Status.ToLower().Trim().Equals(EnumStatusLot.Pause.ToString().ToLower().Trim()));
                Invoice invoice;
                
                if (lotPauseEnds.Any())
                {
                    foreach (var lot in lotPauseEnds)
                    {
                        lot.Status = EnumStatusLot.Passed.ToString();
                        var losers = lot.CustomerLots?.ToList();
                        await SetLoser(losers);
                        _cacheService.UpdateLotStatus(lot.Id, lot.Status);
                        string lotGroupName = $"lot-{lot.Id}";
                        await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPriceforSercetBiddingAuto", "Phiên đã kết thúc!");
                        lot.ActualEndTime = DateTime.UtcNow;
                        await _unitOfWork.SaveChangeAsync();
                    }
                }

                if (lotEnds.Any())
                {
                    foreach (var lot in lotEnds)
                    {
                        var bidPriceWinner = await GetWinnerInEndLot(lot);
                        if (bidPriceWinner != null)
                        {
                            lot.CurrentPrice = bidPriceWinner.CurrentPrice;
                            lot.CustomerLots.First(x => x.CustomerId == bidPriceWinner?.CustomerId).CurrentPrice = bidPriceWinner.CurrentPrice;
                            lot.CustomerLots.First(x => x.CustomerId == bidPriceWinner?.CustomerId).IsWinner = true;
                            lot.CustomerLots.First(x => x.CustomerId == bidPriceWinner?.CustomerId).IsInvoiced = true;
                            lot.CustomerLots.First(x => x.CustomerId == bidPriceWinner?.CustomerId).Status = EnumCustomerLot.CreateInvoice.ToString();
                            var fee = bidPriceWinner.CurrentPrice * await _foorFeeService.GetPercentFloorFeeOfLot((float)bidPriceWinner.CurrentPrice);
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
                            //tao mới historystatuscustomerlot
                            var historyStatusCustomerLot = new HistoryStatusCustomerLot()
                            {
                                CustomerLotId = lot.CustomerLots.First(x => x.CustomerId == bidPriceWinner?.CustomerId).Id,
                                Status = EnumCustomerLot.CreateInvoice.ToString(),
                                CurrentTime = DateTime.UtcNow,
                            };
                            _customerLotService.CreateHistoryCustomerLot(historyStatusCustomerLot);
                            await _unitOfWork.InvoiceRepository.AddAsync(invoice);
                            lot.Status = EnumStatusLot.Sold.ToString();
                            //tao noti cho nguoi thang
                            var notification = new Notification
                            {
                                Title = $"Bidding win in Lot {lot.Id}",
                                Description = $" You won in lot {lot.Id} and system auto created invoice for you.",
                                Is_Read = false,
                                NotifiableId = invoice.Id,
                                AccountId = bidPriceWinner.Customer.AccountId,
                                CreationDate = DateTime.UtcNow,
                                Notifi_Type = "CreateInvoice",
                                ImageLink = lot.Jewelry.ImageJewelries.FirstOrDefault()?.ImageLink

                            };
                            await _unitOfWork.NotificationRepository.AddAsync(notification);
                            await _notificationHub.Clients.Group(bidPriceWinner.Customer.AccountId.ToString()).SendAsync("NewNotificationReceived", "Có thông báo mới!");

                            var losers = lot.CustomerLots.Where(x => x.CustomerId != bidPriceWinner.CustomerId).ToList();
                            await SetLoser(losers);
                            //Người THUA Nè
                            //nếu thua cập nhật is winner == false
                            //refurn và cập nhật is refurn == true
                            //cập nhật status customerlot là refuned 
                            //tao moi history status customer trạng thái refurn 
                        }
                        else
                        {

                            lot.Status = EnumStatusLot.Passed.ToString();           
                            var losers = lot.CustomerLots.Where(x => x.LotId == lot.Id).ToList();
                            if(losers != null)
                            {
                                await SetLoser(losers);
                            } 
                        }
                        _cacheService.UpdateLotStatus(lot.Id, lot.Status);
                        lot.ActualEndTime = lot.EndTime;
                        string lotGroupName = $"lot-{lot.Id}";
                        await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPriceforSercetBiddingAuto", "Phiên đã kết thúc!");                       
                        await _unitOfWork.SaveChangeAsync();

                    }
                }
            }
        }
        //public async Task AutoBidAsync()
        //{
        //    using (var scope = _serviceProvider.CreateScope())
        //    {
        //        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        //        var _customerLotService = scope.ServiceProvider.GetRequiredService<ICustomerLotService>();
        //        var _cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

        //        // x.AutoBids.FirstOrDefault().IsActive == true 
        //        var customerLotActives = await _unitOfWork.CustomerLotRepository.GetAllAsync(x => x.Lot.Status == EnumStatusLot.Auctioning.ToString());
        //        //get hight price right now
        //        var customerLots = new List<CustomerLot>();
        //        foreach (var item in customerLotActives)
        //        {
        //            string redisKey1 = $"BidPrice:{item.LotId}";
        //            //lay ra highest bidPrice
        //            var topBidders = _cacheService.GetSortedSetDataFilter<BidPrice>(redisKey1, l => l.LotId == item.LotId);

        //            var highestBidOfLot = topBidders.FirstOrDefault()?.CurrentPrice.Value ?? item.Lot.StartPrice.GetValueOrDefault();

        //            var currentPrice = highestBidOfLot;

        //            if (item.AutoBids.Any(x => x.IsActive == true && x.MinPrice <= currentPrice && x.MaxPrice >= currentPrice))
        //            {
        //                customerLots.Add(item);
        //            }
        //        }

        //        try
        //        {

        //            foreach (var player in customerLots)
        //            {

        //                if (await _customerLotService.CheckTimeAutoBid(player.Id))
        //                {
        //                    string redisKey1 = $"BidPrice:{player.LotId}";
        //                    //lay ra highest bidPrice
        //                    var topBidders = _cacheService.GetSortedSetDataFilter<BidPrice>(redisKey1, l => l.LotId == player.LotId);
        //                    var highestBidOfLot = topBidders.FirstOrDefault()?.CurrentPrice.Value ?? player.Lot.StartPrice.GetValueOrDefault();

        //                    Console.WriteLine($"HighestBidOfLot after initialization: {highestBidOfLot}");

        //                    var currentPriceOfPlayer = topBidders.OrderByDescending(x => x.CurrentPrice).FirstOrDefault(x => x.CustomerId == player.CustomerId);

        //                    var autobidAvaiable = player.AutoBids?.FirstOrDefault(x => x.IsActive == true && x.MinPrice <= highestBidOfLot && x.MaxPrice >= highestBidOfLot);
        //                    if ((currentPriceOfPlayer != null && currentPriceOfPlayer.CurrentPrice.Value >= highestBidOfLot) || highestBidOfLot == null)
        //                    {
        //                        continue;
        //                    }

        //                    if (player.Lot == null || player.Customer == null || highestBidOfLot == null)
        //                    {
        //                        continue;
        //                    }

        //                    //tìm ra autobid phù hợp với autobid có tg thực hiện giữa mỗi lần auto
        //                    if (autobidAvaiable != null)
        //                    {
        //                        // th chưa ai đặt 
        //                        var bidPriceFuture = highestBidOfLot + (player.Lot.BidIncrement * autobidAvaiable.NumberOfPriceStep);
        //                        //nếu giá đấu tương lai lớn hơn giá bán cuối của lot thì ko làm gì cả
        //                        if (bidPriceFuture > player.Lot.FinalPriceSold)
        //                        {
        //                            continue;
        //                        }
        //                        var (isFuturePrice, price) = await _customerLotService.CheckBidPriceTop((float)bidPriceFuture, highestBidOfLot, autobidAvaiable);

        //                        // Nếu giá đấu hiện tại cao hơn, hãy tiếp tục kiểm tra với giá hiện tại
        //                        if (!isFuturePrice && price != null)
        //                        {
        //                            bidPriceFuture = (float)(price + player.Lot.BidIncrement * autobidAvaiable.NumberOfPriceStep);
        //                            (isFuturePrice, price) = await _customerLotService.CheckBidPriceTop((float)bidPriceFuture, highestBidOfLot, autobidAvaiable);
        //                        }

        //                        // Nếu giá đấu hiện tại bé hơn, thì lấy bidPriceFuture luôn, không cânf kt lại
        //                        if (isFuturePrice && price != null)
        //                        {

        //                            //kiểm tra bidLimit của customer có đủ điều kiện để đấu với giá này hay không
        //                            if (player.Customer.PriceLimit >= bidPriceFuture)
        //                            {
        //                                var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(player.CustomerId);
        //                                var firstName = customer.FirstName;
        //                                var lastname = customer.LastName;
        //                                //luu vao hang doi
        //                                player.CurrentPrice = bidPriceFuture;

        //                                BiddingInputDTO bidData = new BiddingInputDTO
        //                                {
        //                                    CurrentPrice = bidPriceFuture,
        //                                    BidTime = DateTime.UtcNow
        //                                };

        //                                string lotGroupName = $"lot-{player.LotId}";
        //                                var bidPriceStream = _cacheService.AddToStream((int)player.Lot.Id, bidData, (int)player.CustomerId);
        //                                await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPriceForStaff", bidPriceStream.CustomerId, firstName, lastname, bidPriceStream.CurrentPrice, bidPriceStream.BidTime);
        //                                await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPrice", bidPriceStream.CustomerId, bidPriceStream.CurrentPrice, bidPriceStream.BidTime);
        //                                await _unitOfWork.SaveChangeAsync();
        //                                await _hubContext.Clients.Group(lotGroupName).SendAsync("AutoBid", "AutoBid End Time");
        //                                TimeSpan delayTime = TimeSpan.FromMinutes(autobidAvaiable.TimeIncrement.Value);
        //                                await Task.Delay(delayTime);
        //                            }
        //                        }
        //                        if (!isFuturePrice && price == null)
        //                        {
        //                            continue;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            throw;
        //        }
        //    }
        //}

        private async Task HandleLoserLot(int lotId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _floorFeeService = scope.ServiceProvider.GetRequiredService<IFoorFeePercentService>();
                var _cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                string redisKey = $"BidPrice:{lotId}";
                var bidPrices = _cacheService.GetSortedSetDataFilter<BidPrice>(redisKey, l => l.LotId == lotId);
                await _unitOfWork.BidPriceRepository.AddRangeAsync(bidPrices);
                var lot = await _unitOfWork.LotRepository.GetByIdAsync(lotId);

                //lay ra list customerLot theo  lotId 
                var customerlots = await _unitOfWork.CustomerLotRepository.GetAllAsync(x => x.LotId == lotId);
                if (customerlots != null)
                {
                    List<CustomerLot> listCustomerLot = new List<CustomerLot>();
                    foreach (var customerLot in customerlots)
                    {
                        customerLot.IsWinner = false;

                        //hoan coc cho loser
                        var walletOfLoser = await _unitOfWork.WalletRepository.GetByCustomerId(customerLot.CustomerId);
                        walletOfLoser.AvailableBalance += ((decimal?)customerLot?.Lot?.Deposit ?? 0);
                        walletOfLoser.Balance = walletOfLoser.AvailableBalance ?? 0 + walletOfLoser.FrozenBalance ?? 0;
                        _unitOfWork.WalletRepository.Update(walletOfLoser);
                        customerLot.IsRefunded = true;
                        customerLot.Status = EnumCustomerLot.Refunded.ToString();
                        listCustomerLot.Add(customerLot);

                        var maxBidPriceLoser = await _unitOfWork.BidPriceRepository.GetMaxBidPriceByCustomerIdAndLot(customerLot.CustomerId, lotId);
                        if (maxBidPriceLoser == null)
                        {
                            customerLot.CurrentPrice = 0;
                        }
                        else
                        {
                            customerLot.CurrentPrice = maxBidPriceLoser.CurrentPrice;
                        }

                        _unitOfWork.CustomerLotRepository.Update(customerLot);
                        //lưu history của loser là refunded
                        var historyCustomerlotLoser = new HistoryStatusCustomerLot()
                        {
                            Status = customerLot.Status,
                            CustomerLotId = customerLot.Id,
                            CurrentTime = DateTime.UtcNow,
                        };
                        await _unitOfWork.HistoryStatusCustomerLotRepository.AddAsync(historyCustomerlotLoser);

                        //tao notification cho loser
                        var notificationloser = new Notification
                        {
                            Title = $"Lot {lotId} had been passed",
                            Description = $" Lot {lotId} had been passed and system auto refunded deposit for you",
                            Is_Read = false,
                            NotifiableId = customerLot.Id,  //cusrtomerLot => dẫn tới myBid
                            AccountId = customerLot.Customer.AccountId,
                            CreationDate = DateTime.UtcNow,
                            Notifi_Type = "Refunded",
                            ImageLink = lot.Jewelry.ImageJewelries.FirstOrDefault()?.ImageLink

                            //  ImageLink = lot.Jewelry.ImageJewelries.FirstOrDefault().ImageLink
                        };

                        await _unitOfWork.NotificationRepository.AddAsync(notificationloser);
                        await _notificationHub.Clients.Group(customerLot.Customer.AccountId.ToString()).SendAsync("NewNotificationReceived", "Có thông báo mới!");
                        //cap nhat transaction vi
                        var walletTrasaction = new WalletTransaction
                        {
                            transactionType = EnumTransactionType.RefundDeposit.ToString(),
                            DocNo = customerLot.Id,
                            Amount = customerLot.Lot.Deposit,
                            TransactionTime = DateTime.UtcNow,
                            Status = "Completed",
                            WalletId = customerLot.Customer.Wallet.Id,
                            transactionPerson = customerLot.Customer.Id
                        };
                        await _unitOfWork.WalletTransactionRepository.AddAsync(walletTrasaction);


                        //cap nhat transaction cty
                        var trasaction = new Transaction
                        {
                            TransactionType = EnumTransactionType.RefundDeposit.ToString(),
                            DocNo = customerLot.Id,
                            Amount = customerLot.Lot.Deposit,
                            TransactionTime = DateTime.UtcNow,
                            TransactionPerson = customerLot.CustomerId

                        };
                        await _unitOfWork.TransactionRepository.AddAsync(trasaction);

                    }
                    _unitOfWork.CustomerLotRepository.UpdateRange(listCustomerLot);

                }
                await _unitOfWork.SaveChangeAsync();

            }
        }
    }
}

