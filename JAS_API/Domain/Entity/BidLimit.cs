using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class BidLimit : BaseEntity
    {
        public string? File { get; set; }
        public float? PriceLimit { get; set; }
        public string? Reason { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string? Status { get; set; }
        //
        public virtual Customer? Customer { get; set; }
    }
}
