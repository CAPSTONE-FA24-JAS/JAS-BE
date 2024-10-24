using Application.Interfaces;
using Application.ViewModels.AuctionDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class AuctionController : BaseController
    {
        private readonly IAuctionService _auctionService;

        public AuctionController(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        [HttpGet]
        public async Task<IActionResult> ViewAutions()
        {
            var result = await _auctionService.ViewAutions();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);  
        }


        [HttpGet]
        public async Task<IActionResult> ViewAutionById(int Id)
        {
            var result = await _auctionService.GetAuctionById(Id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetStatusAuction()
        {
            var result = await _auctionService.GetStatusAuction();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAuctionsByStatus(int statusId)
        {
            var result = await _auctionService.GetAuctionByStatus(statusId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAution([FromForm] CreateAuctionDTO createAuctionDTO)
        {
            var result = await _auctionService.CreateAuction(createAuctionDTO);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateAution([FromForm] UpdateAuctionDTO updateAuctionDTO)
        {
            var result = await _auctionService.UpdateAuction(updateAuctionDTO);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPatch]
        public async Task<IActionResult> ApproveAution(int auctionId)
        {
            var result = await _auctionService.ApproveAuction(auctionId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete]
        public async Task<IActionResult> SoftDeleteAuction(int Id)
        {
            var result = await _auctionService.DeleteSolfAuction(Id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
