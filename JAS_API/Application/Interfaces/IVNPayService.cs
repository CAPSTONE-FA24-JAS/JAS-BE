using Application.ServiceReponse;
using Application.ViewModels.VNPayDTOs;
using Domain.Entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IVNPayService
    {
        Task<string> CreatePaymentUrl(HttpContext httpContext, VNPaymentRequestDTO model, WalletTransaction walletTransaction);
        VNPaymentReponseDTO PaymentExecute(IQueryCollection collection);
    }
}
