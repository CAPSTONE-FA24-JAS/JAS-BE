
using Application;
using Application.Interfaces;
using Application.Services;
using Application.ViewModels.CustomerLotDTOs;
using Domain.Entity;
using Infrastructures;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
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
                        await Task.Delay(delay, stoppingToken);
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
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var _customerLotService = scope.ServiceProvider.GetRequiredService<ICustomerLotService>();
                    var _cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

                    var customerLotActives = await _unitOfWork.CustomerLotRepository.GetAllCustomerLotAuctioningAsync();

                    foreach (var item in customerLotActives?? Enumerable.Empty<CustomerLot>())
                    {
                        if (stoppingToken.IsCancellationRequested)
                        {
                            _logger.LogError("AutoBidAsync Operation canceled.");
                            return;
                        }
                        stoppingToken.ThrowIfCancellationRequested();

                        string redisKey1 = $"BidPrice:{item.Lot.Id}";

                        var highestBid = _cacheService.GetHighestPrice<BidPrice>(redisKey1);

                        var highestBidOfLot = (highestBid?.CurrentPrice.Value != null)? highestBid?.CurrentPrice.Value : item.Lot.StartPrice.GetValueOrDefault();

                        var currentPrice = highestBidOfLot;

                        bool checkAutoBid = item.AutoBids.Any(x => x.IsActive == true && x.MinPrice <= currentPrice && x.MaxPrice >= currentPrice);

                        if (checkAutoBid)
                        {
                            var currentPriceOfPlayer = highestBid != null && highestBid.CustomerId == item.CustomerId && highestBid.Status == "Success" && highestBid.CurrentPrice.Value >= highestBidOfLot;
                            if (currentPriceOfPlayer || highestBidOfLot == null)
                            {
                                return;
                            }
                            else
                            {
                                await ProcessAutoBidAsync(item, highestBid, (float)currentPrice, stoppingToken);
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("AutoBidAsync is Operation canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"1111_Exception into AutoBidAsync {ex.Message} ");
            }  

        }

        private async Task ProcessAutoBidAsync(CustomerLot player, BidPrice highestBid, float currentPrice, CancellationToken stoppingToken)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    if (stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogError("ProcessAutoBidAsync Operation canceled.");
                        return;
                    }
                    var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var _customerLotService = scope.ServiceProvider.GetRequiredService<ICustomerLotService>();
                    stoppingToken.ThrowIfCancellationRequested();

                    var autobidAvaiable = player.AutoBids.FirstOrDefault(x => x.IsActive == true && x.MinPrice <= currentPrice && x.MaxPrice >= currentPrice);
                    if (autobidAvaiable != null)
                    {
                        var availableTime = TimeSpan.FromSeconds(autobidAvaiable.TimeIncrement.Value);
                        var currentPriceOfPlayer = highestBid;
                        TimeSpan distanceTime = (currentPriceOfPlayer != null)
                                                    ? (DateTime.UtcNow - currentPriceOfPlayer.BidTime.Value)
                                                    : availableTime + TimeSpan.FromSeconds(1);

                        if (distanceTime.TotalSeconds > availableTime.TotalSeconds)
                        {
                            float bidPriceFuture = (float)(currentPrice + (player.Lot.BidIncrement * autobidAvaiable.NumberOfPriceStep));
                            if (bidPriceFuture > player.Lot.FinalPriceSold)
                            {
                                return;
                            }
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"1111_Exception into PlaceBid {ex.Message} ");
            }
        }

        private async Task PlaceBid(CustomerLot player, float bidPriceFuture)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"1111_Exception into PlaceBid {ex.Message} ");
            }
            
        }
    }
}