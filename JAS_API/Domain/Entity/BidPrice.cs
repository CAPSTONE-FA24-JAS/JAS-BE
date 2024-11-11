using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class BidPrice : BaseEntity
    {
        public float? CurrentPrice { get; set; }
        public DateTime? BidTime { get; set; }
        public string? Status { get; set; }
        public int? CustomerId { get; set; }
        public int? LotId { get; set; }
        //
        public virtual Customer? Customer { get; set; }
        public virtual Lot? Lot { get; set; }
    }
}
