using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class DashBoardController : BaseController
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ITransactionService _transactionService;

        public DashBoardController(IInvoiceService invoiceService, ITransactionService transactionService)
        {
            _invoiceService = invoiceService;
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> TotalRevenue()
        {
            var result = await _transactionService.TotalRevenue();
            return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> TotalRevenueByMonthWithYear(int month, int year)
        {
            var result = await _transactionService.GetRevenueByMonthWithYear(month, year);
            return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
        }


        [HttpGet]
        public async Task<IActionResult> TotalInvoice()
        {
            var result = await _invoiceService.TotalInvoice();
            return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> TotalInvoiceByMonth(int month)
        {
            var result = await _invoiceService.TotalInvoiceByMonth(month);
            return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
        }


        [HttpGet]
        public async Task<IActionResult> TotalTransactionBreakDown()
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> TotalProfit()
        {
            var result = await _transactionService.TotalProfit();
            return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> TotalProfitByMonthWithYear(int month, int year) 
        {
            var result = await _transactionService.TotalProfitByMonth(month, year);
            return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
        }
    }
}
