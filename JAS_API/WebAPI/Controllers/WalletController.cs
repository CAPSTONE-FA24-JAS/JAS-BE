using Application.Interfaces;
using Application.ViewModels.VNPayDTOs;
using Application.ViewModels.WalletDTOs;
using Domain.Entity;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class WalletController : BaseController
    {
        private readonly IWalletService _walletService;
        private readonly IVNPayService _vpnService;
        private readonly IAccountService _accountService;
        private readonly IVNPayService _vNPayService;
        private readonly IWalletTransactionService _walletTransactionService;

        public WalletController(IWalletService walletService, IVNPayService vpnService, IAccountService accountService, IVNPayService vNPayService, IWalletTransactionService walletTransactionService)
        {
            _walletService = walletService;
            _vpnService = vpnService;
            _accountService = accountService;
            _vNPayService = vNPayService;
            _walletTransactionService = walletTransactionService;
        }

        [HttpGet]
        public async Task<IActionResult> CheckBalance(int walletId)
        {
            var result = await _walletService.CheckBalance(walletId);
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
        public async Task<IActionResult> CheckPasswordWallet(int walletId, string password)
        {
            var result = await _walletService.CheckPasswordWallet(walletId,password);
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
        public async Task<IActionResult> CreateWallet(CreateWalletDTO createWalletDTO)
        {
            var result = await _walletService.CreateWallet(createWalletDTO);
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
        public async Task<IActionResult> TopUp(TopUpWalletDTO topUpWalletDTO)
        {
            var vnPayModel = new VNPaymentRequestDTO
            {
                Amount = topUpWalletDTO.Amount,
                CreatedDate = DateTime.UtcNow,
                Description = "Nap tien vao vi",
                FullName = "",
                OrderId = new Random().Next(1000, 100000)
            };
            var transaction = new WalletTransaction()
            {
                transactionType = EnumTransactionType.AddWallet.ToString(),
                DocNo = topUpWalletDTO.WalletId,
            };
            string paymentUrl = _vpnService.CreatePaymentUrl(HttpContext, vnPayModel, transaction);
            return Content(paymentUrl);
        }

        [HttpGet]
        public async Task<IActionResult> PayCallBack()
        {
            var result =  _vpnService.PaymentExecute(Request.Query);
           
            if (result.VnPayResponseCode == "00" || result.Success != null)
            {
                var tranUpdate = await _walletTransactionService.UpdateTransaction(result.OrderId);
                if(!tranUpdate.IsSuccess)
                {
                        return BadRequest(tranUpdate);
                }

                if (tranUpdate.Data is WalletTransaction trans)
                {
                    var walletUpdate = await _walletService.UpdateBanlance((int)trans.DocNo, (decimal)trans.Amount, true);

                    if (!walletUpdate.IsSuccess)
                    {
                        return BadRequest(walletUpdate);
                    }
                }
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> RequestNewWithdraw(RequestWithdrawDTO requestWithdrawDTO)
        {
            var result = await _walletService.RequestWithdraw(requestWithdrawDTO);
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
        public async Task<IActionResult> ApproveRequestNewWithdraw(int transId)
        {
            var result = await _walletService.ApproveRequestWithdraw(transId);
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
