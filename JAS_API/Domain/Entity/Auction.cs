using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Auction : BaseEntity
    {
        public string? Name { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public string? Description { get; set; }
        public string? ImageLink { get; set; }
        public string? Status { get; set; }
        // 
        public virtual IEnumerable<Lot>? Lots { get; set; }
    }
}
