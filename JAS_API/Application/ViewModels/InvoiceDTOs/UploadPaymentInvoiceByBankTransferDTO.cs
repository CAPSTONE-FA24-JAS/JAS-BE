using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.InvoiceDTOs
{
    public class UploadPaymentInvoiceByBankTransferDTO
    {
        public int? InvoiceId { get; set; }
        public IFormFile FileBill { get; set; }
    }
}
