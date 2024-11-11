using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.InvoiceDTOs
{
    public class StatusInvoiceDTO
    {
        public int Id { get; set; }
        public string? Status { get; set; }
        public string? ImageLink { get; set; }
        public DateTime? CurrentDate { get; set; }
        public int? InvoiceId { get; set; }
        //
        
    }
}
