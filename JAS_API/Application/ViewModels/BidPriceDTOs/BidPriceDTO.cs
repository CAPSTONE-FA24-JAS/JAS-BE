﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.BidPriceDTOs
{
    public class BidPriceDTO
    {
        public float? CurrentPrice { get; set; }
        public DateTime BidTime { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int LotId { get; set; }
    }
}
