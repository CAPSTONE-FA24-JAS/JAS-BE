using Application.Interfaces;
using Application;
using Domain.Enums;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WebAPI.Middlewares;
using Domain.Entity;
using Application.Services;
using System.Collections.Concurrent;

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
                    var lotLiveBidding = await _unitOfWork.LotRepository.GetLotsForAutoServiceAsync(EnumLotType.Public_Auction.ToString() , EnumStatusLot.Auctioning.ToString());

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
                var _bidQueue = new ConcurrentQueue<string>();
                var dataAvailable = new TaskCompletionSource<bool>();

                await _cacheService.SubscribeToChannelAsync(pubsubChannel, message =>
                {
                    _bidQueue.Enqueue(message); // Thêm tin nhắn vào hàng đợi
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

                        // Kiểm tra và xử lý các tin nhắn trong hàng đợi
                        while (_bidQueue.TryDequeue(out var message))
                        {
                            if (message == "Newbid")
                            {
                                _logger.LogInformation($"Nhận được tín hiệu Newbid cho Lot {lot.Id}");

                                // Xử lý logic đặt giá (PlaceBid)
                                var result = _cacheService.PlaceBidWithLuaScript(lot.Id);

                                if (result.result)
                                {
                                    var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(result.bidPrice.CustomerId);
                                    var firstName = customer.FirstName;
                                    var lastName = customer.LastName;

                                    // Gửi cập nhật qua SignalR
                                    await _hubContext.Clients.Group(lotGroupName)
                                        .SendAsync("SendTopPrice", result.highestBid);

                                    await _hubContext.Clients.Group(lotGroupName).SendAsync("SendTopPrice", result.highestBid);
                                    //trar về name, giá ĐẤU, thời gian
                                    await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPriceForStaffAfterProcessingStream", result.bidPrice.CustomerId, firstName, lastName, result.bidPrice.CurrentPrice, result.bidPrice.BidTime, result.bidPrice.Status);

                                    await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPriceAfterProcessingStream", result.bidPrice.CustomerId, result.bidPrice.CurrentPrice, result.bidPrice.BidTime, result.bidPrice.Status);

                                    _logger.LogInformation($"Đã xử lý bid từ {firstName} {lastName} cho Lot {lot.Id}");
                                }
                                else
                                {
                                    _logger.LogInformation($"Không có giá hợp lệ trong luồng stream cho Lot {lot.Id}");
                                }
                            }
                            else
                            {
                                _logger.LogInformation($"message:" + message);
                            }
                        }

                        // Tạm dừng để giảm tải CPU
                        await Task.Delay(100);
                    }
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation($"Lot {lot.Id} đã dừng.");
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
                       lotById.Status == EnumStatusLot.Cancelled.ToString();
            }
        }
    }
}
