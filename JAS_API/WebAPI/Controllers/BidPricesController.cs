using Application.Interfaces;
using Application.Services;
using Application.ViewModels.CustomerLotDTOs;
using Application.ViewModels.LotDTOs;
using Domain.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebAPI.Middlewares;

namespace WebAPI.Controllers
{
    public class BidPricesController : BaseController
    {

        private readonly IBidPriceService _bidPriceService;
        private readonly ILotService _lotService;


        public BidPricesController(IBidPriceService bidPriceService, ILotService lotService)
        {
            _bidPriceService = bidPriceService;
            _lotService = lotService;
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


        //pause,close, open lot
        [HttpPut]
        public async Task<IActionResult> OpenAndPauseLotAsync(int lotId, int? status)
        {

            var result = await _bidPriceService.UpdateStatusBid(lotId, status);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> CancelLotAsync(int lotId)
        {

            var result = await _bidPriceService.cancelLot(lotId);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceBidFixedPriceAndSercet(PlaceBidFixedPriceAndSercet placeBidFixedPriceAndSercetDTO)
        {
            var result = await _lotService.PlaceBidFixedPriceAndSercet(placeBidFixedPriceAndSercetDTO);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("place-buy-now")]
        public async Task<IActionResult> PlaceBuyNow([FromBody] PlaceBidBuyNowDTO request)
        {

            var result = await _lotService.PlaceBuyNow(request);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> checkPlacebidForReduceBidding(int customerId, int lotId)
        {

            var result = await _bidPriceService.checkPlacebidForReduceBidding(customerId,lotId);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

    }
}
