using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.InvoiceDTOs
{
    public class SuccessfulDeliveryRequestDTO
    {
        public int? InvoiceId { get; set; }
        public int? Status { get; set; }

        public IFormFile? ImageDelivery {  get; set; }
    }
}
