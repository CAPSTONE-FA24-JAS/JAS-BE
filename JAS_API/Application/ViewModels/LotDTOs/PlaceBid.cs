using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.LotDTOs
{
    public class PlaceBidBuyNowDTO
    {
        public int? CustomerId { get; set; }
        public int? LotId { get; set; }
    }
    public class PlaceBidFixedPriceAndSercet
    {
        public float? CurrentPrice { get; set; }
        public int? CustomerId { get; set; }
        public int? LotId { get; set; }
    }
    
}
