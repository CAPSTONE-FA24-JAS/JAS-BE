using Application.Interfaces;
using Application.ViewModels.ValuationDTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class ValuationsController : BaseController
    {
        private readonly IValuationService _valuationService;

        public ValuationsController(IValuationService valuationService)
        {
            _valuationService = valuationService;
        }

        [HttpPost]
        public async Task<IActionResult> consignAnItem(ConsignAnItemDTO consignAnItem)
        {
            var result = await _valuationService.ConsignAnItem(consignAnItem);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> getValuationsAsync()
        {
            var result = await _valuationService.GetAllAsync();
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> updateStatusConsignItemAsync(int id, int staffId, string status)
        {
            var result = await _valuationService.UpdateStatusAsync(id, staffId, status);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> createPreliminaryPriceAsync(int id, string status, float preliminaryPrice)
        {
            var result = await _valuationService.CreatePreliminaryValuationAsync(id, status, preliminaryPrice);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> getValuationByIdAsync(int valuationId)
        {
            var result = await _valuationService.getPreliminaryValuationByIdAsync(valuationId);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> getPreliminaryValuationsOfSellerAsync(int sellerId)
        {
            var result = await _valuationService.getPreliminaryValuationsOfSellerAsync(sellerId);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }


        [HttpGet]
        public async Task<IActionResult> getPreliminaryValuationByStatusOfSellerAsync(int sellerId, string status)
        {
            var result = await _valuationService.getPreliminaryValuationByStatusOfSellerAsync(sellerId, status);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatusBySellerAsync(int id, string status)
        {
            var result = await _valuationService.UpdateStatusBySellerAsync(id, status);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
