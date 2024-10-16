using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.CustomerLotDTOs
{
    public class BiddingInputDTO
    {
        public int Price { get; set; } = 0;
        public DateTime Timestamp { get; set; }

        public string ConnectionId { get; set; }
    }
}
