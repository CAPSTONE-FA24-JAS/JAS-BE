using Application.Interfaces;
using Application.ViewModels.VNPayDTOs;
using Application.ViewModels.WalletDTOs;
using CloudinaryDotNet.Actions;
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
        private readonly ITransactionService _transactionService;
        private readonly IClaimsService _claimsService;
        private readonly IInvoiceService _invoiceService;
        private readonly ILotService _lotService;
         private readonly ILogger<WalletController> _logger;

        public WalletController(IWalletService walletService, IVNPayService vpnService, IAccountService accountService, IVNPayService vNPayService, IWalletTransactionService walletTransactionService, ITransactionService transactionService, IClaimsService claimsService, IInvoiceService invoiceService, ILogger<WalletController> logger, ILotService lotService)
        {
            _walletService = walletService;
            _vpnService = vpnService;
            _accountService = accountService;
            _vNPayService = vNPayService;
            _walletTransactionService = walletTransactionService;
            _transactionService = transactionService;
            _claimsService = claimsService;
            _invoiceService = invoiceService;
            _lotService = lotService;
            _logger = logger;
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
                transactionPerson = topUpWalletDTO.CustomerId,
                DocNo = topUpWalletDTO.WalletId,
            };
            string paymentUrl = await _vpnService.CreatePaymentUrl(HttpContext, vnPayModel, transaction);
            return Content(paymentUrl);
        }

        //[HttpGet]
        //public async Task<IActionResult> PayCallBack()
        //{
        //    var result =  _vpnService.PaymentExecute(Request.Query);

        //    if (result.VnPayResponseCode == "00" || result.Success != null)
        //    {
        //        var tranUpdate = await _walletTransactionService.UpdateTransaction(result.OrderId);
        //        if (!tranUpdate.IsSuccess)
        //        {
        //            return BadRequest(tranUpdate);
        //        }
        //        if (tranUpdate.Data is WalletTransaction trans)
        //            {
        //                if (trans.transactionType == EnumTransactionType.AddWallet.ToString())
        //                {
        //                    var walletUpdate = await _walletService.UpdateBanlance((int)trans.DocNo, (decimal)trans.Amount, true);

        //                    if (walletUpdate.IsSuccess)
        //                    {
        //                        return Ok(result);
        //                    }

        //                }

        //                if (trans.transactionId == result.OrderId)
        //                {
        //                    var newTrans = new Transaction()
        //                    {
        //                        Amount = trans.Amount,
        //                        DocNo = trans.DocNo,
        //                        TransactionTime = DateTime.UtcNow,
        //                        TransactionType = trans.transactionType
        //                    };
        //                    var transactionResult = await _transactionService.CreateNewTransaction(newTrans);
        //                    if (transactionResult.IsSuccess)
        //                    {
        //                        return Ok(result);
        //                    }
        //                }

        //        }

        //    }
        //    return Ok(result);
        //}

        [HttpGet]
        public async Task<IActionResult> PayCallBack()
        {
            var result = _vpnService.PaymentExecute(Request.Query);
            try
            {
                if (result.VnPayResponseCode == "00" || result.Success != null)
                {
                    var tranUpdate = await _walletTransactionService.UpdateTransaction(result.OrderId);

                    if (!tranUpdate.IsSuccess)
                    {
                        return BadRequest(tranUpdate);
                    }

                    if (tranUpdate.Data != null && tranUpdate.Data is WalletTransaction trans)
                    {

                        if (trans.transactionType == EnumTransactionType.AddWallet.ToString())
                        {
                            var walletUpdate = await _walletService.UpdateBanlance((int)trans.DocNo, (decimal)trans.Amount, true);

                            if (walletUpdate.IsSuccess)
                            {
                                return Ok(result);
                            }
                            else
                            {
                                return BadRequest(walletUpdate);
                            }
                        }

                        if (trans.transactionId == result.OrderId)
                        {
                            var newTrans = new Transaction()
                            {
                                Amount = trans.Amount,
                                DocNo = trans.DocNo,
                                TransactionTime = DateTime.UtcNow,
                                TransactionType = trans.transactionType
                            };

                            var transactionResult = await _transactionService.CreateNewTransaction(newTrans);
                            var invoice = await _invoiceService.GetInvoiceById((int)trans.DocNo);
                            await _walletService.RefundToWalletForUsersAsync(invoice.CustomerLot.Lot);
                            if (transactionResult.IsSuccess)
                            {
                                return Ok(result);
                            }
                            else
                            {
                                return BadRequest(transactionResult);
                            }
                        }
                    }
                    else
                    {
                        if (tranUpdate.Data == null)
                        {
                            _logger.LogWarning("Transaction update returned null data.");
                            return BadRequest("Transaction data is null.");
                        }

                    }
                }
                else
                {
                    return BadRequest("Payment execution failed.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during PayCallBack execution.");
                return StatusCode(500, "Internal server error.");
            }

            return Ok(result);
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
