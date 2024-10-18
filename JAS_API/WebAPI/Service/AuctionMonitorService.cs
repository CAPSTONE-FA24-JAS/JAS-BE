
namespace WebAPI.Service
{
    public class AuctionMonitorService : BackgroundService
    {
        private readonly ILiveBiddingService _liveBiddingService;
        private readonly ILogger<AuctionMonitorService> _logger;

        public AuctionMonitorService(ILiveBiddingService liveBiddingService, ILogger<AuctionMonitorService> logger)
        {
            _liveBiddingService = liveBiddingService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Checking for ended auctions...");

                await _liveBiddingService.CheckLotStartAsync();
                await _liveBiddingService.ChecKLotEndAsync();

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
