using Application.Interfaces;
using Application.Services;
using Application.ViewModels.InvoiceDTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class InvoicesController : BaseController
    {
        private readonly IInvoiceService _invoiceService;

        public InvoicesController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }



        //List invoice for manager
        [HttpGet]
        public async Task<IActionResult> getInvoicesByStatusForManger(int status, int? pageSize, int? pageIndex)
        {
            var result = await _invoiceService.getInvoicesByStatusForManger(status, pageSize, pageIndex);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }


        //chi dinh shipper
        [HttpPut]
        public async Task<IActionResult> AsignShipperAsync(int invoiceId, int shipperId, int status)
        {
            var result = await _invoiceService.AsignShipper(invoiceId, shipperId, status);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }


        //list invoice cua shipper theo status
        [HttpGet]
        public async Task<IActionResult> GetInvoiceByStatusOfShipper(int shipperId, int status, int? pageSize, int? pageIndex)
        {
            var result = await _invoiceService.GetInvoiceByStatusOfShipper(shipperId, status, pageSize, pageIndex);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        //shipper cap nhat anh và status khi giao hang thanh cong
        [HttpPut]
        public async Task<IActionResult> UpdateSuccessfulDeliveryByShipper(SuccessfulDeliveryRequestDTO deliveryDTO)
        {
            var result = await _invoiceService.UpdateSuccessfulDeliveryByShipper(deliveryDTO);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        //manager check va finish don hang
        [HttpPut]
        public async Task<IActionResult> UpdateStatusByInvoiceId(int invoiceId, int status)
        {
            var result = await _invoiceService.UpdateStatus(invoiceId, status);
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
