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

        private readonly IBidPriceService _bidPriceService;


        public BidPricesController(IBidPriceService bidPriceService)
        {
            _bidPriceService = bidPriceService;
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinBid([FromBody] JoinLotRequestDTO request)
        {
            var result = await _bidPriceService.JoinBid(request);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }


        [HttpPost("place-bid")]
        public async Task<IActionResult> PlaceBiding([FromBody] BiddingInputDTO request)
        {

            var result = await _bidPriceService.PlaceBiding(request);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("place-bid-reduceBidding")]
        public async Task<IActionResult> PlaceBidForReduceBidding([FromBody] BiddingInputDTO request)
        {

            var result = await _bidPriceService.PlaceBidForReducedBidding(request);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

    }
}
