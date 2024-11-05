using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.AutoBidDTOs
{
    public class CreateAutoBidDTO
    {
        public float? MinPrice { get; set; }
        public float? MaxPrice { get; set; }
        public float? NumberOfPriceStep { get; set; }
        public DateTime? TimeIncrement { get; set; }
        //public bool? IsActive { get; set; }
        public int? CustomerLotId { get; set; }
    }
}
