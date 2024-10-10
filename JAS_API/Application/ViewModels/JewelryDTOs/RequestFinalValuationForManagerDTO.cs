using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.JewelryDTOs
{
    public class RequestFinalValuationForManagerDTO
    {
        
        public float? StartingPrice { get; set; }
        
        public string? BidForm { get; set; }
        public DateTime? Time_Bidding { get; set; }

        public int? JewelryId { get; set; }
    }
}
