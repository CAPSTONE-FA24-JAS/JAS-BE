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
        byte[] CreateReceiptPDF(Valuation valuation, string? jewelryName, DateTime? recivedDate, string? productRecivedStatus, string? note, string? khoiluong);
        byte[] CreateAuthorizedPDF(Valuation valuation);
    }
}
