using Application.Interfaces;
using Application.Services;
using Application.ViewModels.CustomerLotDTOs;
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
                if (!_shared.connections.ContainsKey(request.ConnectionId))
                {
                    _shared.connections[request.ConnectionId] = new CustomerLotDTO { CustomerId = request.CustomerId };
                    await _hubContext.Clients.All.SendAsync("ReceivedMessage", "admin", $"{request.CustomerId} has joined bidding");
                }
                else
                {
                    await _hubContext.Clients.All.SendAsync("ReceivedMessage", "admin", $"{request.CustomerId} has joined bidding");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error sending message to the hub" });
            }
            return Ok();
        }
    }
}
