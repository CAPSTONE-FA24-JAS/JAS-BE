using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class DashBoardController : BaseController
    {
        private readonly IDashBoardService _dashboardService;

        public DashBoardController(IDashBoardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> TotalRevenue()
        {
            var result = await _dashboardService.TotalRevenue();
            return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> TotalRevenueByMonthWithYear(int month, int year)
        {
            var result = await _dashboardService.GetRevenueByMonthWithYear(month, year);
            return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
        }


        [HttpGet]
        public async Task<IActionResult> TotalInvoice()
        {
            var result = await _dashboardService.TotalInvoice();
            return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> TotalInvoiceByMonth(int month)
        {
            var result = await _dashboardService.TotalInvoiceByMonth(month);
            return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
        }


        //[HttpGet]
        //public async Task<IActionResult> TotalTransactionBreakDown()
        //{
        //    return Ok();
        //}

        //[HttpGet]
        //public async Task<IActionResult> TotalProfit()
        //{
        //    var result = await _transactionService.TotalProfit();
        //    return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
        //}

        //[HttpGet]
        //public async Task<IActionResult> TotalProfitByMonthWithYear(int month, int year)
        //{
        //    var result = await _transactionService.TotalProfitByMonth(month, year);
        //    return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
        //}
        
        [HttpGet]
        public async Task<IActionResult> DashBoardRevenueInYear(int year)
        {
            var result = await _dashboardService.DashBoardRevenueInYear(year);
            return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> DashBoardInvoiceInYear(int year)
        {
            var result = await _dashboardService.DashBoardInvoiceInYear(year);
            return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> TopFiveJewelryAuctions()
        {
            var result = await _dashboardService.GetTopFiveJewelryAuctionsAsync();
            return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
        }
    }
}
