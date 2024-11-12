using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.InvoiceDTOs
{
    public class UpdateAddressToShipInvoice
    {
        public int InvoiceId { get; set; }
        public int AddressToShipId { get; set; }
    }
}
