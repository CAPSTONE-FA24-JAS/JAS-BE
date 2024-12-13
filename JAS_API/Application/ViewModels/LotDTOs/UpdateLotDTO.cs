using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.LotDTOs
{
    public class UpdateLotDTO
    {
    }
    public class BaseUpdateLot
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int? Round { get; set; }
        public int? StaffId { get; set; }
        public int? JewelryId { get; set; }
        public int? AuctionId { get; set; }
        public bool? IsExtend { get; set; }
        public float? Deposit { get; set; }

    }

    public class UpdateLotFixedPriceDTO : BaseLot
    {
        public float? BuyNowPrice { get; set; }
        public bool? HaveFinancialProof { get; set; }

    
    }
    public class UpdateLotSecretAuctionDTO : BaseLot
    {
        public float? StartPrice { get; set; }
        public bool? HaveFinancialProof { get; set; }

    }
    public class UpdateLotPublicAuctionDTO : BaseLot
    {
        public float? StartPrice { get; set; }
        public float? FinalPriceSold { get; set; }
        public float? BidIncrement { get; set; }
        public bool? HaveFinancialProof { get; set; }
        public bool? IsHaveFinalPrice { get; set; }

    }
    public class UpdateLotAuctionPriceGraduallyReducedDTO : BaseLot
    {
        public float? StartPrice { get; set; }
        public float? FinalPriceSold { get; set; }
        public float? BidIncrement { get; set; }
        public bool? HaveFinancialProof { get; set; }
        public int? BidIncrementTime { get; set; }
    }
}
