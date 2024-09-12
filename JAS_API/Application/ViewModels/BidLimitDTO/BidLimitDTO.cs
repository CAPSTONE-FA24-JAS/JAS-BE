using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.BidLimitDTO
{
    public class BidLimitDTO
    {
        public int? Id { get; set; }
        public string? File { get; set; }
        public float? PriceLimit { get; set; }
        public int? AccountId { get; set; }
        public string? AccountName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string? Status { get; set; }
    }
}
