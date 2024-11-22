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
        private readonly ITransactionService _transactionService;
        private readonly IClaimsService _claimsService;
        private readonly IInvoiceService _invoiceService;
        private readonly ILotService _lotService;
        private readonly ICustomerLotService _customerLotService;
        private readonly ILogger<WalletController> _logger;

        public WalletController(IWalletService walletService, IVNPayService vpnService, IAccountService accountService, IVNPayService vNPayService, IWalletTransactionService walletTransactionService, ITransactionService transactionService, IClaimsService claimsService, IInvoiceService invoiceService, ILogger<WalletController> logger, ILotService lotService, ICustomerLotService customerLotService)
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
            _customerLotService = customerLotService;
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
            var result = await _walletService.CheckPasswordWallet(walletId, password);
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
                TransactionTime = DateTime.UtcNow,
                DocNo = topUpWalletDTO.WalletId,
            };
            string paymentUrl = await _vpnService.CreatePaymentUrl(HttpContext, vnPayModel, transaction);
            return Content(paymentUrl);
        }

        [HttpGet]
        public async Task<IActionResult> PayCallBack()
        {
            var result = _vpnService.PaymentExecute(Request.Query);
            try
            {
                if (result.VnPayResponseCode == "00" || result.Success != null)
                {
                    if (result.DocNo != null)
                    {
                            var invoice = await _invoiceService.GetInvoiceById((int)result.DocNo);
                            invoice.Status = EnumCustomerLot.Paid.ToString();
                            invoice.CustomerLot.Status = EnumCustomerLot.Paid.ToString();
                            var newTrans = new Transaction()
                            {
                                Amount = invoice.TotalPrice,
                                DocNo = invoice.Id,
                                TransactionTime = DateTime.UtcNow,
                                TransactionType = EnumTransactionType.BuyPay.ToString(),
                                TransactionPerson = invoice.CustomerId,
                            };
                            var historyStatusCustomerLot = new HistoryStatusCustomerLot()
                            {
                                CustomerLotId = invoice.CustomerLot.Id,
                                Status = EnumCustomerLot.Paid.ToString(),
                                CurrentTime = DateTime.UtcNow,
                            };
                            _customerLotService.CreateHistoryCustomerLot(historyStatusCustomerLot);
                            var transactionResult = await _transactionService.CreateNewTransaction(newTrans);
                            if (transactionResult.IsSuccess)
                            {
                                return Ok(result);
                            }
                            else
                            {
                                return BadRequest(transactionResult);
                            }
                    }
                    else
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
                                var newTrans = new Transaction()
                                {
                                    Amount = trans.Amount,
                                    DocNo = trans.DocNo,
                                    TransactionTime = DateTime.UtcNow,
                                    TransactionType = trans.transactionType,
                                    TransactionPerson = trans.transactionPerson,
                                };
                                var transactionResult = await _transactionService.CreateNewTransaction(newTrans);
                                if (walletUpdate.IsSuccess && transactionResult.IsSuccess)
                                {
                                    return Ok(result);
                                }
                                else
                                {
                                    return BadRequest(walletUpdate);
                                }
                            }
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

        [HttpGet]
        public async Task<IActionResult> ViewListRequestWithdrawForManagerment()
        {
            var result = await _walletService.GetListRequestWithdrawForManagerment();
            return (result.IsSuccess) ? Ok(result) : BadRequest(result);
        }

        [HttpPatch]
        public async Task<IActionResult> ApproveRequestNewWithdraw(int requestId)
        {
            var result = await _walletService.ApproveRequestWithdraw(requestId);
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
