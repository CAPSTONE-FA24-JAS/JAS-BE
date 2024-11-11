using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.BidLimitDTOs
{
    public class BidLimitDTO
    {
        public int? Id { get; set; }
        public string? File { get; set; }
        public float? PriceLimit { get; set; }
        public int? CustomerId { get; set; }
        public int? StaffId { get; set; }
        public string? StaffName { get; set; }
        public string? CustomerName { get; set; }
        public string? Reason { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string? Status { get; set; }
        
    }
}
