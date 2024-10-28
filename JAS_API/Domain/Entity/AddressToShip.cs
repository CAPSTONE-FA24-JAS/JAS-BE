using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class AddressToShip : BaseEntity
    {
        public string? AddressLine { get; set; }
        public bool? IsDefault { get; set; }
        public int? CustomerId { get; set; }
        //public int? WardId { get; set; }
        //
        public virtual Customer? Customer { get; set; }
        //public virtual Ward? Ward { get; set; }
        public virtual IEnumerable<Invoice>? Invoices { get; set; }
        
    }
}
