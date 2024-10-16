﻿using Application.ViewModels.AccountDTOs;
using Application.ViewModels.AuctionDTOs;
using Application.ViewModels.JewelryDTOs;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.LotDTOs
{
    public class LotDTO
    {
        public int? Id { get; set; }
        public string? Status { get; set; }
        public float? Deposit { get; set; }
        public int? FloorFeePercent { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public bool? IsExtend { get; set; }
        public bool? HaveFinancialProof { get; set; }
        public string? LotType { get; set; }

        public int? SellerId { get; set; }
        public int? StaffId { get; set; }
        public int? JewelryId { get; set; }
        public int? AuctionId { get; set; }
        //
        public virtual SellerDTO? Seller { get; set; }
        public virtual StaffDTO? Staff { get; set; }
        public virtual AuctionDTO? Auction { get; set; }
        public virtual JewelryDTO? Jewelry { get; set; }
        //public virtual IEnumerable<BidPrice>? BidPrices { get; set; }
        //public virtual IEnumerable<CustomerLot>? CustomerLots { get; set; }
        //public virtual IEnumerable<Watching>? Watchings { get; set; }
    }
    public class LotFixedPriceDTO : LotDTO
    {
        public float? BuyNowPrice { get; set; }
    }

    public class LotSecretAuctionDTO : LotDTO
    {
        public float? StartPrice { get; set; }
        public float? FinalPriceSold { get; set; }
        
    }
    public class LotPublicAuctionDTO : LotDTO
    {
        public float? StartPrice { get; set; }
        public float? CurrentPrice { get; set; }
        public float? FinalPriceSold { get; set; }
        public float? BidIncrement { get; set; }
        public float? BuyNowPrice { get; set; }

        
    }
    public class LotAuctionPriceGraduallyReducedDTO : LotDTO
    {
        public float? StartPrice { get; set; }
        public float? FinalPriceSold { get; set; }
        public float? BidIncrement { get; set; }

    }
}