using Application.Interfaces;
using Application.ViewModels.BidLimitDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class BidLimitController : BaseController
    {
        private readonly IBidLimitService _bidLimitService;

        public BidLimitController(IBidLimitService bidLimitService)
        {
            _bidLimitService = bidLimitService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBidLimit([FromForm] CreateBidLimitDTO createBidLimitDTO)
        {
            var result = await _bidLimitService.CreateNewBidLimit(createBidLimitDTO);
            if(result.IsSuccess != true)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }
        [HttpGet]
        public async Task<IActionResult> ViewAllBidLimit()
        {
            var result = await _bidLimitService.ViewListBidLimit();
            if (result.IsSuccess != true)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }
        [HttpGet]
        public async Task<IActionResult> ViewAllBidLimitByAccount(int accountId)
        {
            var result = await _bidLimitService.ViewBidLimitByAccount(accountId);
            if (result.IsSuccess != true)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }
        [HttpGet]
        public async Task<IActionResult> ViewAllBidLimitById(int Id)
        {
            var result = await _bidLimitService.ViewBidLimitById(Id);
            if (result.IsSuccess != true)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }
        [HttpGet]
        public async Task<IActionResult> ViewStatusBidLimt()
        {
            var result = await _bidLimitService.GetStatusBidLimt();
            if (result.IsSuccess != true)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateStatusBidLimit(int status, int Id)
        {
            var result = await _bidLimitService.UpdateStatus(status, Id);
            if (result.IsSuccess != true)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }
    }
}
