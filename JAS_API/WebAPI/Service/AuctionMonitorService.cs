﻿
namespace WebAPI.Service
{
    public class AuctionMonitorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AuctionMonitorService> _logger;

        public AuctionMonitorService(IServiceProvider serviceProvider, ILogger<AuctionMonitorService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var liveBiddingService = scope.ServiceProvider.GetRequiredService<LiveBiddingService>();
                        await liveBiddingService.CheckLotStartAsync();
                        await liveBiddingService.AutoBidAsync();
                        await liveBiddingService.ChecKLotEndAsync();
                        await liveBiddingService.ChecKLotEndReducedBiddingAsync();
                        //await liveBiddingService.CheckLotBuyNowAsync();                
                        await liveBiddingService.CheckLotFixedPriceAsync();
                        await liveBiddingService.CheckLotSercetAsync();
                        await liveBiddingService.ChecKAuctionEndAsync();
                        


                    }
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                // Log lỗi và tiếp tục
                _logger.LogError(ex, "Error in AuctionMonitorService");
            }

        }
    }
}
