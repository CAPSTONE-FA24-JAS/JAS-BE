using Application.Interfaces;
using Application.Repositories;
using Application.Services;
using Application.ViewModels.AutoBidDTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class CustomerLotsController : BaseController
    {
        private readonly ICustomerLotService _customerLotService;
        private readonly IAutoBidService _autoBidService;

        public CustomerLotsController(ICustomerLotService customerLotService, IAutoBidService autoBidService)
        {
            _customerLotService = customerLotService;
            _autoBidService = autoBidService;
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

        [HttpGet]
        public async Task<IActionResult> GetMyBidByCustomerLotIdAsync(int customerLotId)
        {
            var result = await _customerLotService.GetMyBidByCustomerLotId(customerLotId);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> SetAutoBid(CreateAutoBidDTO createAutoBidDTO)
        {
            var result = await _autoBidService.SetAutoBid(createAutoBidDTO);
            return (result.IsSuccess == true)? Ok(result) : BadRequest(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetWinnerForLotAsync(int lotId)
        {
            var result = await _customerLotService.GetWinnerForLot(lotId);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAutoBidByCustomerLot(int customerLotId)
        {
            var result = await _autoBidService.GetAutoBisByCustomerdLot(customerLotId);
            return (result.IsSuccess == true) ? Ok(result) : BadRequest(result);
        }
    }
}
