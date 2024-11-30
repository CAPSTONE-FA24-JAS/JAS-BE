using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IGeneratePDFService
    {
        byte[] CreateReceiptPDF(Valuation valuation, DateTime? recivedDate, string? productRecivedStatus, string? note);
        byte[] CreateAuthorizedPDF(Valuation valuation);
    }
}
