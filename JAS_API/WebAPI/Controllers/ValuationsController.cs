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
    }
}
