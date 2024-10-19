using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    
    public class PaymentController : BaseController
    {
        private readonly IVNPayService _vNPayService;

        public PaymentController(IVNPayService vNPayService)
        {
            _vNPayService = vNPayService;
        }

        //[HttpPost]
        //public async Task<IActionResult> ViewCustomerLotByLotId()
        //{
        //    var result = await _vNPayService.CreatePaymentUrl();
        //    if (!result.IsSuccess)
        //    {
        //        return BadRequest(result);
        //    }
        //    return Ok(result);
        //}
    }
}
