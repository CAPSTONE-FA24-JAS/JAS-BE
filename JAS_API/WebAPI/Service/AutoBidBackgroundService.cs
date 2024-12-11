
using Application;
using Application.Interfaces;
using Application.Services;
using Application.ViewModels.CustomerLotDTOs;
using Domain.Entity;
using Infrastructures;
using Microsoft.AspNetCore.SignalR;
using WebAPI.Middlewares;

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
            using (var cts = new CancellationTokenSource())
            {
                int delay = 1000;
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            await AutoBidAsync(stoppingToken);
                        }
                        await Task.Delay(delay, stoppingToken);
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogInformation("Operation canceled.");
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred in AutoBidBackgroundService");
                        await Task.Delay(delay, stoppingToken);
                    }
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

                var customerLotActives = await _unitOfWork.CustomerLotRepository.GetAllCustomerLotAuctioningAsync();

                foreach (var item in customerLotActives)
                {
                    stoppingToken.ThrowIfCancellationRequested();

                    string redisKey1 = $"BidPrice:{item.Lot.Id}";
                    var topBidders = _cacheService.GetSortedSetDataFilter<BidPrice>(redisKey1, l => l.LotId == item.Lot.Id);

                    var highestBidOfLot = topBidders.FirstOrDefault()?.CurrentPrice.Value ?? item.Lot.StartPrice.GetValueOrDefault();
                    var currentPrice = highestBidOfLot;

                    if (item.AutoBids.Any(x => x.IsActive == true && x.MinPrice <= currentPrice && x.MaxPrice >= currentPrice))
                    {
                        await ProcessAutoBidAsync(item, topBidders, currentPrice, stoppingToken);
                    }
                }
            }
        }

        private async Task ProcessAutoBidAsync(CustomerLot player, IEnumerable<BidPrice> topBidders, float currentPrice, CancellationToken stoppingToken)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var _customerLotService = scope.ServiceProvider.GetRequiredService<ICustomerLotService>();
                    stoppingToken.ThrowIfCancellationRequested();

                    var autobidAvaiable = player.AutoBids.FirstOrDefault(x => x.IsActive == true && x.MinPrice <= currentPrice && x.MaxPrice >= currentPrice);
                    if (autobidAvaiable != null)
                    {
                        var availableTime = TimeSpan.FromSeconds(autobidAvaiable.TimeIncrement.Value);
                        var currentPriceOfPlayer = topBidders.OrderByDescending(x => x.CurrentPrice).FirstOrDefault(x => x.CustomerId == player.CustomerId && x.Status == "Success");
                        TimeSpan distanceTime = (currentPriceOfPlayer != null)
                                                    ? (DateTime.UtcNow - currentPriceOfPlayer.BidTime.Value)
                                                    : availableTime + TimeSpan.FromSeconds(1);

                        if (distanceTime.TotalSeconds > availableTime.TotalSeconds)
                        {
                            float bidPriceFuture = (float)(currentPrice + (player.Lot.BidIncrement * autobidAvaiable.NumberOfPriceStep));
                            var (isFuturePrice, price) = await _customerLotService.CheckBidPriceTop(bidPriceFuture, currentPrice, autobidAvaiable);

                            if (!isFuturePrice && price != null)
                            {
                                bidPriceFuture = (float)(price + player.Lot.BidIncrement * autobidAvaiable.NumberOfPriceStep);
                                (isFuturePrice, price) = await _customerLotService.CheckBidPriceTop(bidPriceFuture, currentPrice, autobidAvaiable);
                            }

                            if (isFuturePrice && price != null)
                            {
                                if (player.Customer.PriceLimit >= bidPriceFuture)
                                {
                                    await PlaceBid(player, bidPriceFuture);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error processing player {player.Id}");
            }
        }

        private async Task PlaceBid(CustomerLot player, float bidPriceFuture)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

                var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(player.CustomerId);
                var firstName = customer.FirstName;
                    var lastname = customer.LastName;

                    player.CurrentPrice = bidPriceFuture;

                    BiddingInputDTO bidData = new BiddingInputDTO
                    {
                        CurrentPrice = bidPriceFuture,
                        BidTime = DateTime.UtcNow
                    };

                    string lotGroupName = $"lot-{player.Lot.Id}";
                    var bidPriceStream = _cacheService.AddToStream((int)player.Lot.Id, bidData, (int)player.CustomerId);

                    await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPriceForStaff", bidPriceStream.CustomerId, firstName, lastname, bidPriceStream.CurrentPrice, bidPriceStream.BidTime);
                    await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPrice", bidPriceStream.CustomerId, bidPriceStream.CurrentPrice, bidPriceStream.BidTime);
                    await _unitOfWork.SaveChangeAsync();
                    await _hubContext.Clients.Group(lotGroupName).SendAsync("AutoBid", "AutoBid End Time");

                    var lot = _cacheService.GetLotById(player.Lot.Id);
                    if (lot != null && lot.IsExtend == true) 
                    {
                        TimeSpan extendTime = lot.EndTime.Value - bidPriceStream.BidTime.Value;
                        if (extendTime.TotalSeconds < 10)
                        {
                            DateTime endTime = lot.EndTime.Value.AddSeconds(10);
                            _cacheService.UpdateLotEndTime(player.Lot.Id, endTime);
                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", player.Lot.Id, endTime);
                        }
                        else
                        {
                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", player.Lot.Id, lot.EndTime);
                        }
                    }
                    else if (lot != null && lot.EndTime.HasValue)
                    {
                        DateTime endTime = lot.EndTime.Value;
                        await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", player.Lot.Id, endTime);
                    }
            }
        }
    }
}