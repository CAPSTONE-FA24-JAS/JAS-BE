using Application.Interfaces;
using Application.Services;
using Application.ViewModels.InvoiceDTOs;
using Application.ViewModels.VNPayDTOs;
using Application.ViewModels.WalletDTOs;
using Castle.Core.Resource;
using Domain.Entity;
using Domain.Enums;
using iTextSharp.text;
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

        //Get Detail of invoice by invoice id
        [HttpGet]
        public async Task<IActionResult> GetDetailInvoice(int invoiceId)
        {
            var result = await _invoiceService.GetInvoiceDetail(invoiceId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateAddressToShipForInvoice(UpdateAddressToShipInvoice model)
        {
            var result = await _invoiceService.UpdateAddressToshipForInvoice(model);
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
        public async Task<IActionResult> FinishInvoiceByManager(int invoiceId, int status)
        {
            var result = await _invoiceService.FinishInvoiceByManager(invoiceId, status);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }


        //list invoice cua customer theo status. neu staus = null thi list all
        [HttpGet]
        public async Task<IActionResult> getInvoicesByStatusForCustomerAsync(int customerId, int? status, int? pageSize, int? pageIndex)
        {
            var result = await _invoiceService.getInvoicesByStatusForCustomer(customerId, status, pageSize, pageIndex);
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
        public async Task<IActionResult> paymentInvoiceByVnPay(PaymentInvoiceByVnPayDTO model)
        {
            var result = await _invoiceService.PaymentInvoiceByVnPay(model);
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
