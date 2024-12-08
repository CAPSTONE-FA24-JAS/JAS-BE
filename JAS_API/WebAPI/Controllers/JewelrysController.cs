using Application.Interfaces;
using Application.Services;
using Application.ViewModels.ArtistDTOs;
using Application.ViewModels.JewelryDTOs;
using Microsoft.AspNetCore.Components;
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


        [HttpGet]
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

        [HttpGet]
        public async Task<IActionResult> GetJewelryNoLotAsync(int? pageSize, int? pageIndex)
        {
            var result = await _jewelryService.GetJewelryNoLotAsync(pageSize, pageIndex);
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
        public async Task<IActionResult> RequestOTPForAuthorizedBySellerAsync(int valuationId, int sellerId)
        {
            var result = await _jewelryService.RequestOTPForAuthorizedBySellerAsync(valuationId, sellerId);
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
        public async Task<IActionResult> VerifyOTPForAuthorizedBySellerAsync(int valuationId, int sellerId, string opt)
        {
            var result = await _jewelryService.VerifyOTPForAuthorizedBySellerAsync(valuationId, sellerId, opt);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetJewelryByCategoryAsync(int categoryId, int? pageSize, int? pageIndex)
        {
            var result = await _jewelryService.GetJewelryByCategoryAsync(categoryId,pageSize, pageIndex);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }        }

        [HttpGet]
        public async Task<IActionResult> GetJewelryByArtistAsync(int artistId, int? pageSize, int? pageIndex)
        {
            var result = await _jewelryService.GetJewelryByArtistAsync(artistId, pageSize, pageIndex);
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
        public async Task<IActionResult> UpdateJewelry([FromForm]UpdateJewelryDTO model)
        {
            var result = await _jewelryService.UpdateJewelryAsync(model);
            return (result.IsSuccess)?Ok(result):BadRequest(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteJewelry(int jewelryId)
        {
            var result = await _jewelryService.DeleteJewelryAsync(jewelryId);
            return (result.IsSuccess) ? Ok(result) : BadRequest(result);
        }
        

        [HttpGet]
        public async Task<IActionResult> ViewJewelryIsSoldOut()
        {
            var result = await _jewelryService.GetJewelrysIsSoldOut();
            return (result.IsSuccess) ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> ViewEnumColorShapphies()
        {
            var result = await _jewelryService.GetEnumColorsShape();
            return (result.IsSuccess) ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> ViewEnumColorDiamonds()
        {
            var result = await _jewelryService.GetEnumColorsDiamond();
            return (result.IsSuccess) ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> ViewEnumCuts()
        {
            var result = await _jewelryService.GetEnumCuts();
            return (result.IsSuccess) ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> ViewEnumClarities()
        {
            var result = await _jewelryService.GetEnumClarities();
            return (result.IsSuccess) ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult>GetDetailJewelryAsync(int jewelryId)
        {
            var result = await _jewelryService.getJewelryByIdAsync(jewelryId);
            return (result.IsSuccess) ? Ok(result) : BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> RejectByManagerAsync(int jewelryId, int status)
        {
            var result = await _jewelryService.RejectByManagerAsync(jewelryId, status);
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
        public async Task<IActionResult> RejectByOwnerAsync(int jewelryId, int status, string reason)
        {
            var result = await _jewelryService.RejectByOwnerAsync(jewelryId, status, reason);
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
        public async Task<IActionResult> CancelByMangerToNoAuctionAsync(int jewelryId,string reason)
        {
            var result = await _jewelryService.CancelByMangerToNoAuctionAsync(jewelryId, reason);
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
