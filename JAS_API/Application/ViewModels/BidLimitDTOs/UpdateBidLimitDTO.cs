using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.BidLimitDTOs
{
    public class UpdateBidLimitDTO
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public float? PriceLimit { get; set; }
        public string? Reason { get; set; }
        public int? StaffId { get; set; }
    }
}
