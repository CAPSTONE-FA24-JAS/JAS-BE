
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
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var liveBiddingService = scope.ServiceProvider.GetRequiredService<LiveBiddingService>();

                    var tasks = new List<Task>
            {
                RunTaskWithLogging(() => liveBiddingService.CheckLotStartAsync(), "CheckLotStartAsync"),
                RunTaskWithLogging(() => liveBiddingService.ChecKLotEndAsync(), "ChecKLotEndAsync"),
                RunTaskWithLogging(() => liveBiddingService.ChecKLotEndReducedBiddingAsync(), "ChecKLotEndReducedBiddingAsync"),
                RunTaskWithLogging(() => liveBiddingService.CheckLotFixedPriceAsync(), "CheckLotFixedPriceAsync"),
                RunTaskWithLogging(() => liveBiddingService.CheckLotSercetAsync(), "CheckLotSercetAsync"),
                RunTaskWithLogging(() => liveBiddingService.ChecKAuctionEndAsync(), "ChecKAuctionEndAsync")
            };

                    await Task.WhenAll(tasks);
                }

                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }

        private async Task RunTaskWithLogging(Func<Task> taskFunc, string taskName)
        {
            try
            {
                await taskFunc();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {taskName}");
            }
        }

    }
}
