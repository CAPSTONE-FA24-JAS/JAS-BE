using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels
{
    public class InvoiceDTO
    {
        public string? Status { get; set; }
         
        public float? TotalPrice { get; set; }
        
        public int? PaymentMethodId { get; set; }
        public int? AddressToShipId { get; set; }
       
        public int? ShipperId { get; set; }
    }
}
