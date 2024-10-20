using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.VNPayDTOs;
using Domain.Entity;
using iTextSharp.xmp.impl;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Application.Services
{
    public class VNPayService : IVNPayService
    {
        private readonly IConfiguration _configuration;
        private readonly IWalletTransactionService _walletTransactionService;

        public VNPayService(IConfiguration configuration, IWalletTransactionService walletTransactionService)
        {
            _configuration = configuration;
            _walletTransactionService = walletTransactionService;
        }

        public string CreatePaymentUrl(HttpContext httpContext, VNPaymentRequestDTO model, WalletTransaction walletTransaction)
        {
            var tick = DateTime.Now.Ticks.ToString();

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _configuration["VnPay:vnp_Version"]);
            vnpay.AddRequestData("vnp_Command", _configuration["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _configuration["VnPay:vnp_TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());

            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _configuration["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.Utils.GetIpAddress(httpContext));
            vnpay.AddRequestData("vnp_Locale", _configuration["VnPay:Locale"]);

            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + model.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _configuration["VnPay:PaymentBackReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", tick);

            //tao transaction tam
            walletTransaction.transactionId = tick;
            walletTransaction.Status = "Pending";
            walletTransaction.Amount = model.Amount;
            walletTransaction.CreationDate = model.CreatedDate;
            _walletTransactionService.CreateNewTransaction(walletTransaction);

            var paymentUrl = vnpay.CreateRequestUrl(_configuration["VnPay:vnp_Url"], _configuration["VnPay:vnp_HashSecret"]);
            return paymentUrl;
        }

        public VNPaymentReponseDTO PaymentExecute(IQueryCollection collection)
        {
            var vnpayreponse = new VNPaymentReponseDTO();
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in collection)
            {
                if(!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash = collection.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            var vnp_TrasactionTime = vnpay.GetResponseData("vnp_PayDate");
            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _configuration["VnPay:vnp_HashSecret"]);

            if (!checkSignature)
            {
                vnpayreponse.Success = false;
                return vnpayreponse;
            }

            
            vnpayreponse.PaymentMethod = "VnPay";
            vnpayreponse.OrderDescription = vnp_OrderInfo;
            vnpayreponse.OrderId = vnp_orderId.ToString();
            vnpayreponse.TransactionId = vnp_TransactionId.ToString();
            vnpayreponse.Token = vnp_SecureHash;
            vnpayreponse.VnPayResponseCode = vnp_ResponseCode;
            DateTime TransactionTime;
            if (DateTime.TryParseExact(vnp_TrasactionTime, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out TransactionTime))
            {
                vnpayreponse.CreatedDate = TransactionTime;
            }
            return vnpayreponse;
        }
    }
}
