using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class StatusInvoice : BaseEntity
    {
        public string? Status { get; set; }
        public string? ImageLink { get; set; }
        public DateTime? CurrentDate { get; set; }
        public int? InvoiceId { get; set; }
        //
        public virtual Invoice? Invoice { get; set; }
    }
}
