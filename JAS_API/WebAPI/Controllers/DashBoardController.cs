﻿using Application.Interfaces;
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
        
        [HttpGet]
        public async Task<IActionResult> TopFiveSellersAsync()
        {
            var result = await _dashboardService.GetTopFiveSellersAsync();
            return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> TopFiveBuyersAsync()
        {
            var result = await _dashboardService.GetTopFiveBuyersAsync();
            return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> TotalAccountsAsync()
        {
            var result = await _dashboardService.TotalUser();
            return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> TotalAccountActiveAsync()
        {
            var result = await _dashboardService.TotalUserActive();
            return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> TotalCustomersAsync()
        {
            var result = await _dashboardService.TotalCustomer();
            return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> TotalCustomersActiveAsync()
        {
            var result = await _dashboardService.TotalCustomerActive();
            return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> TotalRevenueInvoiceAsync()
        {
            var result = await _dashboardService.TotalRevenueInvoice();
            return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> TotalInvoiceByStatus()
        {
            var result = await _dashboardService.TotalInvoiceByStatus();
            return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
        }
    }
}
