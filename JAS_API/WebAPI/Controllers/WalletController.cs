using Application.Interfaces;
using Application.ViewModels.VNPayDTOs;
using Application.ViewModels.WalletDTOs;
using Domain.Entity;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using static QRCoder.PayloadGenerator;

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
                    if (result.DocNo != -1)
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
                            return await StatusPage("Success", true);
                        }
                        else
                        {
                            return await StatusPage("Failed", false);
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
                                    return await StatusPage("Success", true);
                                }
                                else
                                {
                                    return await StatusPage("Failed", false);
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




        [HttpGet]
        public async Task<IActionResult> ViewListRequestWithdrawForManagerment()
        {
            var result = await _walletService.GetListRequestWithdrawForManagerment();
            return (result.IsSuccess) ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> ViewListRequestWithdrawForCustomer(int customerId)
        {
            var result = await _walletService.GetListRequestWithdrawForCustomer(customerId);
            return (result.IsSuccess) ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> RequestNewWithdraw(RequestWithdrawDTO requestWithdrawDTO)
        {
            var result = await _walletService.RequestWithdraw(requestWithdrawDTO);
            return (result.IsSuccess) ? Ok(result) : BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> CancelRequestNewWithdrawByCustomer(int customerId, int requestId)
        {
            var result = await _walletService.CancelRequestWithdrawByCustomer(customerId, requestId);
            return (result.IsSuccess) ? Ok(result) : BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> CancelRequestNewWithdrawByStaff(int requestId)
        {
            var result = await _walletService.RejectRequestWithdrawByStaff(requestId);
            return (result.IsSuccess) ? Ok(result) : BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> ProcessRequestNewWithdrawByStaff(int requestId)
        {
            var result = await _walletService.ProgressRequestWithdraw(requestId);
            return (result.IsSuccess) ? Ok(result) : BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> ApproveRequestNewWithdraw(int requestId)
        {
            var result = await _walletService.ApproveRequestWithdraw(requestId);
            return (result.IsSuccess) ? Ok(result) : BadRequest(result);
        }

        [HttpGet("reponsestatuspagetranfer")]
        public async Task<IActionResult> ReponseStatusPageTranfer(bool isSuccessfull)
        {
            if (isSuccessfull == true)
            {
                return Ok(true);
            }
            else
            {
                return BadRequest(false);
            }
        }

        [HttpGet("statuspage")]
        public async Task<IActionResult> StatusPage(string status, bool returnStatus)
        {
            string templatePageRedirect = $@"
        <!DOCTYPE html>
        <html lang=""en"">
        <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title>Thông Báo Thành Công</title>
            <style>
                body {{
                    font-family: 'Arial', sans-serif;
                    margin: 0;
                    padding: 0;
                    background-color: #f7f7f7;
                    display: flex;
                    justify-content: center;
                    align-items: center;
                    height: 100vh;
                }}

                .container {{
                    text-align: center;
                    padding: 40px 30px;
                    background-color: #ffffff;
                    border-radius: 15px;
                    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                    max-width: 400px;
                    width: 90%;
                    border: 2px solid #e6c300;
                }}

                .container h1 {{
                    font-size: 28px;
                    color: #3cbcb4; /* Xanh ngọc */
                    margin-bottom: 20px;
                }}

                .container p {{
                    font-size: 16px;
                    color: #555555;
                    margin-bottom: 30px;
                }}

                button {{
                    background-color: #e6c300; /* Vàng */
                    color: white;
                    border: none;
                    padding: 12px 20px;
                    font-size: 16px;
                    font-weight: bold;
                    border-radius: 5px;
                    cursor: pointer;
                    transition: all 0.3s ease;
                    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.2);
                }}

                button:hover {{
                    background-color: #d4af37; /* Vàng đậm hơn */
                    transform: scale(1.05);
                }}

                .logo {{
                    background-color: #3cbcb4;
                    width: 60px;
                    height: 60px;
                    display: inline-block;
                    border-radius: 50%;
                    line-height: 60px;
                    text-align: center;
                    color: white;
                    font-size: 24px;
                    font-weight: bold;
                    margin-bottom: 20px;
                    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.2);
                }}
            </style>
        </head>
        <body>
            <div class=""container"">
                <div class=""logo"">💎</div>
                <h1>Chúc mừng!</h1>
                <p>Hành động của bạn đã được thực hiện <b>{status}</b>.</p>

                <button onclick=""openDeeplink(simplemeditation://result-payment?isSuccess={returnStatus.ToString().ToLower()})"">Comback App</button>
            </div>

            <script>
                function redirectToPage() {{
                    // Đặt URL của trang cần chuyển đến
                    window.location.href = ""openDeeplink(simplemeditation://result-payment?isSuccess={returnStatus.ToString().ToLower()})"";
                }}
            </script>
        </body>
        </html>
    ";

            return Content(templatePageRedirect, "text/html");
        }
    }
}
