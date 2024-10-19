using Application.Interfaces;
using Application.Repositories;
using Application.ViewModels.VNPayDTOs;
using Application.ViewModels.WalletDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace WebAPI.Controllers
{
    public class WalletController : BaseController
    {
        private readonly IWalletService _walletService;
        private readonly IVNPayService _vpnService;
        private readonly IAccountService _accountService;

        public WalletController(IWalletService walletService, IVNPayService vpnService, IAccountService accountService)
        {
            _walletService = walletService;
            _vpnService = vpnService;
            _accountService = accountService;
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
                FullName = "danh ne",
                OrderId = new Random().Next(1000, 100000)
            };
            return Redirect(_vpnService.CreatePaymentUrl(HttpContext, vnPayModel));

        }

        [HttpPost]
        public async Task<IActionResult> PayCallBack()
        {
            var result = await _vpnService.PaymentExecute(Request.Query);
            if (result.IsSuccess)
            {
                if (result.Data is VNPaymentReponseDTO reponseDTO)
                {
                    if (reponseDTO.VnPayResponseCode == "00")
                    {
                        //cap nhat vi 

                        return Ok(result);
                    }
                }
            }
            return BadRequest(result);
        }
    }
}
