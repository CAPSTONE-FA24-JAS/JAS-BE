using Application.Interfaces;
using Application.Services;
using Application.ViewModels.CustomerLotDTOs;
using Domain.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebAPI.Middlewares;

namespace WebAPI.Controllers
{
    public class BidPricesController : BaseController
    {
        private readonly IHubContext<BiddingHub> _hubContext;
        private readonly ICacheService _cacheService;
        private readonly ShareDB _shared;


        public BidPricesController(IHubContext<BiddingHub> hubContext, ICacheService cacheService, ShareDB shared)
        {
            _hubContext = hubContext;
            _cacheService = cacheService;
            _shared = shared;
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinBid([FromBody] JoinLotRequestDTO request)
        {
            try
            {
                string lotGroupName = $"lot-{request.LotId}";
                if (!_shared.connections.ContainsKey(request.ConnectionId))
                {
                    _shared.connections[request.ConnectionId] = new CustomerLotDTO 
                    { CustomerId = request.CustomerId ,
                      LotId = request.LotId 
                    };

                    await _hubContext.Groups.AddToGroupAsync(request.ConnectionId, lotGroupName);
                    await _hubContext.Clients.Groups(lotGroupName).SendAsync("JoinLot", "admin", $"{request.CustomerId} has joined lot {request.LotId}");                 
                }
                else
                {
                    await _hubContext.Clients.Groups(lotGroupName).SendAsync("JoinLot", "admin", $"{request.CustomerId} has joined lot {request.LotId}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error sending message to the hub" });
            }
            return Ok();
        }


        [HttpPost("place-bid")]
        public async Task<IActionResult> PlaceBiding([FromBody] BiddingInputDTO request)
        {
            if (_shared.connections.TryGetValue(request.ConnectionId, out CustomerLotDTO conn))
            {
                var bidData = new BidPrice
                {
                    CurrentPrice = request.Price,
                    BidTime = request.Timestamp,
                    CustomerId = conn.CustomerId,
                    LotId = conn.LotId,

                };

                // Lưu dữ liệu đấu giá vào Redis
                _cacheService.SetSortedSetData<BidPrice>("BidPrice", bidData, request.Price);

                // Truy xuất bảng xếp hạng giảm dần theo giá đấu từ Redis
                var topBidders = _cacheService.GetSortedSetData<BidPrice>("BidPrice");
                var highestBid = topBidders.FirstOrDefault();

                string lotGroupName = $"lot-{conn.LotId}";
                //trar về name, giá ĐẤU, thời gian
                await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPrice", conn.CustomerId, request.Price, request.Timestamp);

                await _hubContext.Clients.Group(lotGroupName).SendAsync("SendTopPrice", highestBid.CurrentPrice, highestBid.BidTime);

                // Lấy thời gian kết thúc từ Redis
                var lot = _cacheService.GetLotById(conn.LotId);
                if (lot.EndTime.HasValue)
                {
                    DateTime endTime = lot.EndTime.Value;

                    //10s cuối
                    TimeSpan extendTime = endTime - request.Timestamp;

                    // Nếu còn dưới 10 giây thì gia hạn thêm 10 giây
                    if (extendTime.TotalSeconds < 10)
                    {
                        endTime = endTime.AddSeconds(10);
                        _cacheService.UpdateLotEndTime(conn.LotId, endTime);
                    }
                }
                    
                return Ok(topBidders);
            }
            return BadRequest(new { message = "Connection not found" });

        }

    }
}
