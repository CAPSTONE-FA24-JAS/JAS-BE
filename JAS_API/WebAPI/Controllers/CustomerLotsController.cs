using Application.Interfaces;
using Application.Repositories;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class CustomerLotsController : BaseController
    {
        private readonly ICustomerLotService _customerLotService;

        public CustomerLotsController(ICustomerLotService customerLotService)
        {
            _customerLotService = customerLotService;
        }

        //lay ra list my bid chua dau hoac dang dau cua customer status registed = 1
        [HttpGet]
        public async Task<IActionResult> GetBidsOfCustomerAsync(int? customerIId, int? status, int? pageIndex, int? pageSize)
        {
            var result = await _customerLotService.GetBidsOfCustomer(customerIId, status, pageIndex, pageSize);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //list my past bid by customerId
        [HttpGet]
        public async Task<IActionResult> GetPastBidOfCustomerAsync(int customerIId, [FromQuery] List<int>? status, int? pageIndex, int? pageSize)
        {
            var result = await _customerLotService.GetPastBidOfCustomer(customerIId, status, pageIndex, pageSize);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

    }
}
