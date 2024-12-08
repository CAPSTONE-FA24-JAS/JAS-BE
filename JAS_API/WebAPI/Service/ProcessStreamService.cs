using Application.Interfaces;
using Application;
using Domain.Enums;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WebAPI.Middlewares;
using Domain.Entity;
using Application.Services;

namespace WebAPI.Service
{
    public class ProcessStreamService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AuctionMonitorService> _logger;

        private readonly IHubContext<BiddingHub> _hubContext;

        public ProcessStreamService(IServiceProvider serviceProvider, ILogger<AuctionMonitorService> logger, IHubContext<BiddingHub> hubContext)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var runningTasks = new List<Task>();
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var _cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                    var lotLiveBidding = await _unitOfWork.LotRepository.GetAllAsync(x => x.LotType == EnumLotType.Public_Auction.ToString() && x.Status ==  EnumStatusLot.Auctioning.ToString());

                    if(lotLiveBidding == null)
                    {
                        await Task.Delay(100, stoppingToken);
                    }
                    else
                    {
                        foreach (var lot in lotLiveBidding)
                        {
                            // Khởi chạy từng tác vụ ProcessBids độc lập và thêm vào danh sách runningTasks
                            var task = Task.Run(() => ProcessBids(lot), stoppingToken);
                            runningTasks.Add(task);
                        }

                        // Xóa các tác vụ đã hoàn thành khỏi danh sách để tránh tràn bộ nhớ
                        runningTasks.RemoveAll(t => t.IsCompleted);
                    }                   
                }
                await Task.Delay(1000, stoppingToken);
            }
        }

        public async Task ProcessBids(Lot lot)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                string lotGroupName = $"lot-{lot.Id}";
                var pubsubChannel = $"channel-{lot.Id}";
                var dataAvailable = new TaskCompletionSource<bool>();

                await _cacheService.SubscribeToChannelAsync(pubsubChannel, message =>
                {
                    if (message == "Newbid")
                    {
                        dataAvailable.TrySetResult(true); // Signal that data is available
                    }
                });
                try
                {
                    while (true)
                    {
                        if (await IsLotFinishedAsync(lot))
                        {
                            _logger.LogInformation($"Lot {lot.Id} đã kết thúc.");
                            break;
                        }

                        var timeout = Task.Delay(1000); // 1 giây
                        var signal = await Task.WhenAny(dataAvailable.Task, timeout);
                        if (signal == timeout)
                        {
                            _logger.LogInformation("Timeout reached, no new bids.");
                            continue; // Tiếp tục vòng lặp, không làm gì nếu không có tín hiệu
                        }


                        _logger.LogInformation("Received new bid, processing...");
                        //xu ly kiem gia gia dau từ hàng đợi stream bằng lua script rôi lưu vào sortset
                        var result = _cacheService.PlaceBidWithLuaScript(lot.Id);

                        if (result.result == false)
                        {
                            //  _logger.LogInformation($"Không còn giá nào trong Stream cho Lot {lot.Id}");
                            // Đợi 100ms trước khi kiểm tra lại
                              dataAvailable = new TaskCompletionSource<bool>();  // Reset the signal
                             //  await dataAvailable.Task;
                           // await Task.Delay(700);
                            continue;
                        }
                        else
                        {
                            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(result.bidPrice.CustomerId);


                            var firstName = customer.FirstName;
                            var lastname = customer.LastName;
                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendTopPrice", result.highestBid);
                            //trar về name, giá ĐẤU, thời gian
                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPriceForStaffAfterProcessingStream", result.bidPrice.CustomerId, firstName, lastname, result.bidPrice.CurrentPrice, result.bidPrice.BidTime, result.bidPrice.Status);

                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPriceAfterProcessingStream", result.bidPrice.CustomerId, result.bidPrice.CurrentPrice, result.bidPrice.BidTime, result.bidPrice.Status);
                        }
                       
                    }
                }catch(TaskCanceledException)
                {
                    _logger.LogInformation("Task was canceled successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deserializing resultJson: {ex.Message}");
                }

            }
        }


        private async Task<bool> IsLotFinishedAsync(Lot lot)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                
                var lotById = await _unitOfWork.LotRepository.GetByIdAsync(lot.Id);

                return lotById.Status == EnumStatusLot.Passed.ToString() ||
                       lotById.Status == EnumStatusLot.Sold.ToString() ||
                       lotById.Status == EnumStatusLot.Canceled.ToString();
            }
        }
    }
}
