﻿using Application.ViewModels.WalletDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.VNPayDTOs
{
    public class VNPaymentRequestDTO
    {
        public int OrderId { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public float Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public int DocNo { get; set; }

    }
    public class VNPaymentReponseDTO
    {
        public bool Success { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderDescription { get; set; }
        public string OrderId { get; set; }
        public int DocNo { get; set; }
        public string PaymentId { get; set; }
        public string TransactionId { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }
        public DateTime CreatedDate { get; set; }

    }


    public class PaymentInvoiceByWalletDTO : RequestWithdrawDTO
    {
        public int InvoiceId { get; set; }
    }
    public class PaymentInvoiceByBankTransferDTO
    {
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
        //public int CreditCardId { get; set; }
    }
    public class PaymentInvoiceByVnPayDTO
    {
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
    }
}
