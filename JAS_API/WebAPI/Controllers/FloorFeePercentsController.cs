using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.Services;
using Application.ViewModels.FloorFeeDTOs;
namespace WebAPI.Controllers
{
    public class FloorFeePercentsController : BaseController
    {
        private readonly IFoorFeePercentService _foorFeePercentService;

        public FloorFeePercentsController(IFoorFeePercentService foorFeePercentService)
        {
            _foorFeePercentService = foorFeePercentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFloorFeesAsync()
        {
            var result = await _foorFeePercentService.GetFloorFeesAsync();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFloorFeesAsync(UpdateFloorFeeDTO updateFloorFeeDTO)
        {
            var result = await _foorFeePercentService.UpdateFloorFeesAsync(updateFloorFeeDTO);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
    }

    
}
