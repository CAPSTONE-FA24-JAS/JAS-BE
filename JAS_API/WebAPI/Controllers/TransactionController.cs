using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    
    public class TransactionController : BaseController
    {
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly ITransactionService _transactionService;

        public TransactionController(IWalletTransactionService walletTransactionService, ITransactionService transactionService)
        {
            _walletTransactionService = walletTransactionService;
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> ViewTransactionsByCustomer(int customerId)
        {
            var result = await _walletTransactionService.ViewWalletTransactionsByCustomerId(customerId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet]
        public async Task<IActionResult> FilterViewTransactionsByCustomer(int customerId, int transTypeId)
        {
            var result = await _walletTransactionService.FilterWalletTransactionsOfCustomerByTransType(customerId, transTypeId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewTransactionType()
        {
            var result = await _walletTransactionService.ViewTransactionType();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewTransactionsOfCompanyByTransType(int transTypeId)
        {
            var result = await _transactionService.GetAllTransactionByTransType(transTypeId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewTransactionsOfCompany()
        {
            var result = await _transactionService.GetAllTransaction();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewRevenueOfCompanyByMonth(int month, int year)
        {
            var result = await _transactionService.GetRevenueByMonth(month, year);
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
