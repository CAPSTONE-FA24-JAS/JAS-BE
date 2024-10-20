using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.LotDTOs
{
    public class CreateLotDTO
    {
        public int LotTypeValue { get; set; }
    }
    public class BaseLot
    {
        public string? Title { get; set; }
    }
  
    public class CreateLotFixedPriceDTO : BaseLot
    {
        public float? Deposit { get; set; }
        public float? BuyNowPrice { get; set; }
        public bool? IsExtend { get; set; }
        public bool? HaveFinancialProof { get; set; }

        public int? StaffId { get; set; }
        public int? JewelryId { get; set; }
        public int? AuctionId { get; set; }
    }
    public class CreateLotSecretAuctionDTO : BaseLot
    {
        public float? StartPrice { get; set; }
        public float? FinalPriceSold { get; set; }
        public float? Deposit { get; set; }
        public bool? IsExtend { get; set; }
        public bool? HaveFinancialProof { get; set; }
        
        public int? StaffId { get; set; }
        public int? JewelryId { get; set; }
        public int? AuctionId { get; set; }
    }
    public class CreateLotPublicAuctionDTO : BaseLot
    {
        public float? StartPrice { get; set; }
        public float? FinalPriceSold { get; set; }
        public float? BidIncrement { get; set; }
        public float? Deposit { get; set; }
        public bool? IsExtend { get; set; }
        public bool? HaveFinancialProof { get; set; }
        
        public int? StaffId { get; set; }
        public int? JewelryId { get; set; }
        public int? AuctionId { get; set; }
    }
    public class CreateLotAuctionPriceGraduallyReducedDTO : BaseLot
    {
        public float? StartPrice { get; set; }
        public float? FinalPriceSold { get; set; }
        public float? BidIncrement { get; set; }
        public float? Deposit { get; set; }
        public bool? IsExtend { get; set; }
        public bool? HaveFinancialProof { get; set; }

        public int? StaffId { get; set; }
        public int? JewelryId { get; set; }
        public int? AuctionId { get; set; }
    }


}
