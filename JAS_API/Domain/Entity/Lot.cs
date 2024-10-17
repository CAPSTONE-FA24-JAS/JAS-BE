using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Lot : BaseEntity
    {
        public float? StartPrice { get; set; }
        public float? CurrentPrice { get; set; }
        public float? FinalPriceSold { get; set; }
        public string? Status { get; set; }
        public float? BidIncrement { get; set; }
        public float? Deposit { get; set; }
        public float? BuyNowPrice { get; set; }
        public int? FloorFeePercent { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public bool? IsExtend { get; set; }
        public bool? HaveFinancialProof { get; set; }
        public string? LotType { get; set; }

        public int? SellerId { get; set; }
        public int? StaffId { get; set; }
        public int? JewelryId { get; set; }
        public int? AuctionId { get; set; }
        //
        public virtual Customer? Seller { get; set; }  
        public virtual Staff? Staff { get; set; }
        public virtual Auction? Auction { get; set; }
        public virtual Jewelry? Jewelry { get; set; }
        public virtual IEnumerable<BidPrice>? BidPrices { get; set; }
        public virtual IEnumerable<CustomerLot>? CustomerLots { get; set; }
        public virtual IEnumerable<Watching>? Watchings { get; set; }
    }
}
