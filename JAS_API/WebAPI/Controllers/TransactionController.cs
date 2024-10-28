using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    
    public class TransactionController : BaseController
    {
        private readonly IWalletTransactionService _walletTransactionService;

        public TransactionController(IWalletTransactionService walletTransactionService)
        {
            _walletTransactionService = walletTransactionService;
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
    }
}
