using Application.Interfaces;
using Application.Services;
using Application.ViewModels.ArtistDTOs;
using Application.ViewModels.JewelryDTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class JewelrysController : BaseController
    {

        private readonly IJewelryService _jewelryService;

        public JewelrysController(IJewelryService jewelryService)
        {
            _jewelryService = jewelryService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFinalValuationAsync([FromForm]CreateFinalValuationDTO finalValuation)
        {
            var result = await _jewelryService.CreateJewelryAsync(finalValuation);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }


        [HttpPost]
        public async Task<IActionResult> GetJewelryAsync(int? pageSize, int? pageIndex)
        {
            var result = await _jewelryService.GetJewelryAsync(pageSize, pageIndex);
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
        public async Task<IActionResult> RequestFinalValuationForManagerAsync(RequestFinalValuationForManagerDTO requestDTO)
        {
            var result = await _jewelryService.RequestFinalValuationForManagerAsync(requestDTO);
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
        public async Task<IActionResult> UpdateStatusByManagerAsync(int jewelryId, int status)
        {
            var result = await _jewelryService.UpdateStatusByManagerAsync(jewelryId, status);
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
        public async Task<IActionResult> RequestOTPForAuthorizedBySellerAsync(int jewelryId, int sellerId)
        {
            var result = await _jewelryService.RequestOTPForAuthorizedBySellerAsync(jewelryId, sellerId);
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
        public async Task<IActionResult> VerifyOTPForAuthorizedBySellerAsync(int jewelryId, int sellerId, string opt)
        {
            var result = await _jewelryService.VerifyOTPForAuthorizedBySellerAsync(jewelryId, sellerId, opt);
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
