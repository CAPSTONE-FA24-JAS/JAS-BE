
using Application;
using Application.Interfaces;
using Application.Services;
using Application.Utils;
using Domain.Entity;
using Domain.Enums;
using Infrastructures;
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
                var maxEndTime = _cacheService.GetMaxEndTimeFormSortedSetOfLot();
                var lotLiveBidding = _cacheService.GetHashLots(l => l.Status == EnumHelper.GetEnums<EnumStatusLot>().FirstOrDefault(x => x.Value == 2).Name);
                int? auctionId = 0;
                foreach (var lot in lotLiveBidding)
                {
                    var endTime = lot.EndTime;
                    if (endTime.HasValue && DateTime.UtcNow > endTime.Value)
                    {
                         await EndLot(lot.Id, endTime.Value);
                    }
                    auctionId = lot.AuctionId;
                    if(auctionId == null)
                    {
                        throw new Exception("AuctionId null");
                    }
                }
                //luu endTime max vao actual auction
                var auction = await _unitOfWork.AuctionRepository.GetByIdAsync(auctionId);
                if (auction != null)
                {
                    auction.ActualEndTime = maxEndTime;
                    _unitOfWork.AuctionRepository.Update(auction);
                    await _unitOfWork.SaveChangeAsync();
                }
            }
                
            
        }

        public async Task CheckLotStartAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                var lotWithStartTime = _cacheService.GetHashLots(l => l.Status == EnumHelper.GetEnums<EnumStatusLot>().FirstOrDefault(x => x.Value == 1).Name);

                foreach (var lot in lotWithStartTime)
                {
                    var startTime = lot.StartTime;
                    if (startTime.HasValue && DateTime.UtcNow >= startTime.Value)
                    {
                        // Nếu đã đến thời gian bắt đầu, chuyển trạng thái lot sang 'Auctioning'
                        await StartLot(lot.Id);
                    }
                }
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
                lot.Status = EnumHelper.GetEnums<EnumStatusLot>().FirstOrDefault(x => x.Value == 2).Name;
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
                    // Truy xuất dữ liệu bid prices từ redis rồi lưu vào sql
                    var bidPrices = _cacheService.GetSortedSetDataFilter<BidPrice>("BidPrice", l => l.LotId == lot.Id);
                    await _unitOfWork.BidPriceRepository.AddRangeAsync(bidPrices);

                    // nếu lot đó k có ai đấu giá thì đổi qua status passed, cập nhật lên cả redis và sql
                    if (bidPrices.Count == 0)
                    {

                        lot.Status = EnumHelper.GetEnums<EnumStatusLot>().FirstOrDefault(x => x.Value == 6).Name;
                        lot.ActualEndTime = endTime;

                        _unitOfWork.LotRepository.Update(lot);

                        _cacheService.UpdateLotStatus(lotId, lot.Status);
                        await _hubContext.Clients.Group(lotGroupName).SendAsync("AuctionEnded", "Phiên đã kết thúc va khong co ai dau gia!");

                    }
                    else
                    {
                        //ngược lại, nếu có bid prices thì truy xuất ra gía max nhất( lưu theo sortedSet và có sắp xếp giảm dần ở hàm get
                        //nên chỉ cần lấy thằng đầu tiên)
                        //lưu status lot là sold
                        //  cập nhật lên cả redis và sql; cập nhật endLot actual, giá bán được
                        var winner = bidPrices.FirstOrDefault();
                        lot.Status = EnumHelper.GetEnums<EnumStatusLot>().FirstOrDefault(x => x.Value == 4).Name;
                        lot.ActualEndTime = endTime;
                        lot.CurrentPrice = winner.CurrentPrice;
                        _unitOfWork.LotRepository.Update(lot);
                        _cacheService.UpdateLotStatus(lotId, lot.Status);


                        await _hubContext.Clients.Group(lotGroupName).SendAsync("AuctionEnded", "Phiên đã kết thúc!", winner.CustomerId, winner.CurrentPrice);

                        //xu ly cho thang thắng
                        //cập nhật customerLot trường IsWinner là true, cập nhật giá đấu được khi đã thắng, cập nhật status lên CreatedInvoice 
                        var winnerCustomerLot = await _unitOfWork.CustomerLotRepository.GetCustomerLotByCustomerAndLot(winner.CustomerId, winner.LotId);
                        winnerCustomerLot.IsWinner = true;
                        winnerCustomerLot.CurrentPrice = winner.CurrentPrice;
                        _unitOfWork.CustomerLotRepository.Update(winnerCustomerLot);
                        //tao invoice cho wwinner
                        var invoice = new Invoice
                        {
                            CustomerId = winner.CustomerId,
                            CustomerLotId = winnerCustomerLot.Id,
                            StaffId = winnerCustomerLot.Lot.StaffId,
                            Price = winner.CurrentPrice,
                            Free = (float?)(winner.CurrentPrice * 0.25),
                            TotalPrice = (float?)(winner.CurrentPrice + winner.CurrentPrice * 0.25 - lot.Deposit),
                        };

                        await _unitOfWork.InvoiceRepository.AddAsync(invoice);

                        winnerCustomerLot.Status = EnumHelper.GetEnums<EnumStatusValuation>().FirstOrDefault(x => x.Value == 2).Name;
                        winnerCustomerLot.IsInvoiced = true;
                        _unitOfWork.CustomerLotRepository.Update(winnerCustomerLot);


                        var historyCustomerlot = new HistoryStatusCustomerLot()
                        {
                            Status = winnerCustomerLot.Status,
                            CustomerLotId = winnerCustomerLot.Id,
                            CurrentTime = DateTime.Now,
                        };
                        await _unitOfWork.HistoryStatusCustomerLotRepository.AddAsync(historyCustomerlot);
                    }



                    //lấy ra những thằng thua theo lot(Bỏ qua giá đấu đầu tiên,group by theo customerId và lotId,Lấy giá trị đầu tiên của mỗi nhóm (distinct)
                    var losers = bidPrices.Skip(1)
                                          .GroupBy(b => new { b.CustomerId, b.LotId })
                                          .Select(g => g.First())
                                          .ToList();

                    //lay ra lisst customerLot theo list customerid vaf lotId
                    var customerLots = _unitOfWork.CustomerLotRepository.GetListCustomerLotByCustomerAndLot(losers);
                    foreach (var loser in customerLots)
                    {
                        loser.IsWinner = false;

                        //hoan coc cho loser
                        var walletOfLoser = await _unitOfWork.WalletRepository.GetByIdAsync(loser.CustomerId);
                        walletOfLoser.Balance = walletOfLoser.Balance + (decimal?)loser.Lot.Deposit;
                        _unitOfWork.WalletRepository.Update(walletOfLoser);
                        loser.IsRefunded = true;


                        //cap nhat transaction vi
                        var walletTrasaction = new WalletTransaction
                        {
                            transactionType = EnumTransactionType.RefundDeposit.ToString(),
                            DocNo = loser.Id,
                            Amount = lot.Deposit,
                            TransactionTime = DateTime.UtcNow,
                            Status = "Completed"
                        };
                        await _unitOfWork.WalletTransactionRepository.AddAsync(walletTrasaction);


                        //cap nhat transaction cty
                        var trasaction = new Transaction
                        {
                            TransactionType = EnumTransactionType.RefundDeposit.ToString(),
                            DocNo = loser.Id,
                            Amount = lot.Deposit,
                            TransactionTime = DateTime.UtcNow,

                        };
                        await _unitOfWork.TransactionRepository.AddAsync(trasaction);
                        await _unitOfWork.SaveChangeAsync();
                    }
                    _unitOfWork.CustomerLotRepository.UpdateRange(customerLots);

                }
                await _unitOfWork.SaveChangeAsync();
            }
                
                
           


        }

               
        }

        
    }

