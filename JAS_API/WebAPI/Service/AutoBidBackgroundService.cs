
using Application.Interfaces;
using Application.ViewModels.CustomerLotDTOs;
using Application;
using Domain.Entity;
using Microsoft.AspNetCore.SignalR;
using WebAPI.Middlewares;
using Domain.Enums;

namespace WebAPI.Service
{
    public class AutoBidBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<BiddingHub> _hubContext;
        private readonly IHubContext<NotificationHub> _notificationHub;
        private readonly ILogger<AutoBidBackgroundService> _logger;

        public AutoBidBackgroundService(IServiceProvider serviceProvider, IHubContext<BiddingHub> hubContext, IHubContext<NotificationHub> notificationHub, ILogger<AutoBidBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
            _notificationHub = notificationHub;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        await AutoBidAsync(stoppingToken);
                    }
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Operation canceled.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in AutoBidBackgroundService");
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
            }
        }
        public async Task AutoBidAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _customerLotService = scope.ServiceProvider.GetRequiredService<ICustomerLotService>();
                var _cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

                // x.AutoBids.FirstOrDefault().IsActive == true 
                var customerLotActives = await _unitOfWork.CustomerLotRepository.GetAllAsync(x => x.Lot.Status == EnumStatusLot.Auctioning.ToString());
                //get hight price right now
                var customerLots = new List<CustomerLot>();
                foreach (var item in customerLotActives)
                {
                    stoppingToken.ThrowIfCancellationRequested();
                    string redisKey1 = $"BidPrice:{item.LotId}";
                    //lay ra highest bidPrice
                    var topBidders = _cacheService.GetSortedSetDataFilter<BidPrice>(redisKey1, l => l.LotId == item.LotId);

                    var highestBidOfLot = topBidders.FirstOrDefault()?.CurrentPrice.Value ?? item.Lot.StartPrice.GetValueOrDefault();

                    var currentPrice = highestBidOfLot;

                    if (item.AutoBids.Any(x => x.IsActive == true && x.MinPrice <= currentPrice && x.MaxPrice >= currentPrice))
                    {
                        customerLots.Add(item);
                    }
                }
                var tasks = customerLots.Select(async player =>
                {
                    try
                    {
                    
                        //foreach (var player in customerLots)
                        //{
                        stoppingToken.ThrowIfCancellationRequested();

                        if (await _customerLotService.CheckTimeAutoBid(player.Id))
                        {
                            string redisKey1 = $"BidPrice:{player.LotId}";
                            //lay ra highest bidPrice
                            var topBidders = _cacheService.GetSortedSetDataFilter<BidPrice>(redisKey1, l => l.LotId == player.LotId);
                            var highestBidOfLot = topBidders.FirstOrDefault()?.CurrentPrice.Value ?? player.Lot.StartPrice.GetValueOrDefault();

                            Console.WriteLine($"HighestBidOfLot after initialization: {highestBidOfLot}");

                            var currentPriceOfPlayer = topBidders.OrderByDescending(x => x.CurrentPrice).FirstOrDefault(x => x.CustomerId == player.CustomerId && x.Status == "Success");

                            var autobidAvaiable = player.AutoBids?.FirstOrDefault(x => x.IsActive == true && x.MinPrice <= highestBidOfLot && x.MaxPrice >= highestBidOfLot);
                            if ((currentPriceOfPlayer != null && currentPriceOfPlayer.CurrentPrice.Value >= highestBidOfLot) || highestBidOfLot == null)
                            {
                                return;
                            }

                            if (player.Lot == null || player.Customer == null || highestBidOfLot == null)
                            {
                                return;
                            }

                            //tìm ra autobid phù hợp với autobid có tg thực hiện giữa mỗi lần auto
                            if (autobidAvaiable != null)
                            {
                                //check time step next
                                TimeSpan availableTime = TimeSpan.FromSeconds(autobidAvaiable.TimeIncrement.Value);

                                TimeSpan distanceTime = (currentPriceOfPlayer != null)
                                                        ? (DateTime.UtcNow - currentPriceOfPlayer.BidTime.Value)
                                                        : availableTime + TimeSpan.FromSeconds(1);
                                if (distanceTime.TotalSeconds > availableTime.TotalSeconds)
                                {
                                    // TH chưa ai đặt 
                                    var bidPriceFuture = highestBidOfLot + (player.Lot.BidIncrement * autobidAvaiable.NumberOfPriceStep);
                                    //nếu giá đấu tương lai lớn hơn giá bán cuối của lot thì ko làm gì cả
                                    if (bidPriceFuture > player.Lot.FinalPriceSold)
                                    {
                                        return;
                                    }
                                    var (isFuturePrice, price) = await _customerLotService.CheckBidPriceTop((float)bidPriceFuture, highestBidOfLot, autobidAvaiable);

                                    // Nếu giá đấu hiện tại cao hơn, hãy tiếp tục kiểm tra với giá hiện tại
                                    if (!isFuturePrice && price != null)
                                    {
                                        bidPriceFuture = (float)(price + player.Lot.BidIncrement * autobidAvaiable.NumberOfPriceStep);
                                        (isFuturePrice, price) = await _customerLotService.CheckBidPriceTop((float)bidPriceFuture, highestBidOfLot, autobidAvaiable);
                                    }

                                    // Nếu giá đấu hiện tại bé hơn, thì lấy bidPriceFuture luôn, không cânf kt lại
                                    if (isFuturePrice && price != null)
                                    {
                                        //kiểm tra bidLimit của customer có đủ điều kiện để đấu với giá này hay không
                                        if (player.Customer.PriceLimit >= bidPriceFuture)
                                        {
                                            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(player.CustomerId);
                                            var firstName = customer.FirstName;
                                            var lastname = customer.LastName;
                                            //luu vao hang doi
                                            player.CurrentPrice = bidPriceFuture;

                                            BiddingInputDTO bidData = new BiddingInputDTO
                                            {
                                                CurrentPrice = bidPriceFuture,
                                                BidTime = DateTime.UtcNow
                                            };

                                            string lotGroupName = $"lot-{player.LotId}";
                                            var bidPriceStream = _cacheService.AddToStream((int)player.Lot.Id, bidData, (int)player.CustomerId);
                                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPriceForStaff", bidPriceStream.CustomerId, firstName, lastname, bidPriceStream.CurrentPrice, bidPriceStream.BidTime);
                                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPrice", bidPriceStream.CustomerId, bidPriceStream.CurrentPrice, bidPriceStream.BidTime);
                                            await _unitOfWork.SaveChangeAsync();
                                            await _hubContext.Clients.Group(lotGroupName).SendAsync("AutoBid", "AutoBid End Time");

                                            var lot = _cacheService.GetLotById(player.Lot.Id);
                                            if (lot.IsExtend == true)
                                            {
                                                if (lot.EndTime.HasValue)
                                                {
                                                    DateTime endTime = lot.EndTime.Value;

                                                    //10s cuối
                                                    TimeSpan extendTime = endTime - bidPriceStream.BidTime.Value;

                                                    //neu round vo tan
                                                    if (lot.Round == null)
                                                    {
                                                        if (extendTime.TotalSeconds < 10)
                                                        {
                                                            endTime = endTime.AddSeconds(10);

                                                            _cacheService.UpdateLotEndTime(player.Lot.Id, endTime);
                                                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", player.Lot.Id, endTime);
                                                        }
                                                        else
                                                        {
                                                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", player.Lot.Id, lot.EndTime);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (extendTime.TotalSeconds < 10)
                                                        {
                                                            if (lot.Round == 0)
                                                            {
                                                                await _hubContext.Clients.Group(lotGroupName).SendAsync("HetRound");
                                                                await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", player.Lot.Id, lot.EndTime);

                                                            }
                                                            else
                                                            {
                                                                endTime = endTime.AddSeconds(10);
                                                                var round = (int)lot.Round - 1;
                                                                _cacheService.UpdateLotEndTime(player.Lot.Id, endTime);
                                                                _cacheService.UpdateLotRound(player.Lot.Id, round);
                                                                await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", player.Lot.Id, endTime);
                                                            }

                                                        }
                                                        else
                                                        {
                                                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", player.Lot.Id, lot.EndTime);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (lot.EndTime.HasValue)
                                                {
                                                    DateTime endTime = lot.EndTime.Value;
                                                    await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", player.Lot.Id, endTime);
                                                }
                                            }

                                        }
                                    }
                                    if (!isFuturePrice && price == null)
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    //}
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Error processing player {player.Id}");
                        throw;
                    }
                });
                await Task.WhenAll(tasks);
            }
        }

    }
}
