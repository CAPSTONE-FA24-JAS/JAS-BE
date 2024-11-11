using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.CustomerLotDTOs
{
    public class BiddingInputDTO
    {
        public float? CurrentPrice { get; set; }
        public DateTime BidTime { get; set; }

        public string ConnectionId { get; set; }

        
    }
}
