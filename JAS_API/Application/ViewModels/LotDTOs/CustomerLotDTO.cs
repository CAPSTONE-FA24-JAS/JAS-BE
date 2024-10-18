using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.LotDTOs
{
    public class CustomerLotDTO
    {
        public bool? IsAutoBid { get; set; }
        public float? CurrentPrice { get; set; }
        public string? Status { get; set; }
        public bool? IsDeposit { get; set; }
        public int? CustomerId { get; set; }
        public int? LotId { get; set; }
        public float? AutoBidPrice { get; set; }
        public float? PriceLimit { get; set; }
        public bool? IsWinner { get; set; }
        public bool? IsRefunded { get; set; }
        public bool? IsInvoiced { get; set; }
        public DateTime? ExpireDateOfBidLimit { get; set; }
    }
}
