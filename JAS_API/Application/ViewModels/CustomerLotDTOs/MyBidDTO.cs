using Application.ViewModels.LotDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.CustomerLotDTOs
{
    public class MyBidDTO
    {
        public int? Id { get; set; }
        public string? Status { get; set; }
        public bool? IsDeposit { get; set; }
        public float? AutoBidPrice { get; set; }
        public float? PriceLimit { get; set; }
        public bool? IsWinner { get; set; }
        public bool? IsRefunded { get; set; }
        public bool? IsInvoiced { get; set; }

        public int? LotId { get; set; }

        public float? yourMaxBidPrice { get; set; }
        public LotDTO LotDTO { get; set; }


    }
}
