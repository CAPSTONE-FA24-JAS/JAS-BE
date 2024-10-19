using Application.ServiceReponse;
using Application.ViewModels.VNPayDTOs;
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
        string CreatePaymentUrl(HttpContext httpContext, VNPaymentRequestDTO model);
        Task<APIResponseModel> PaymentExecute(IQueryCollection collection);
    }
}
