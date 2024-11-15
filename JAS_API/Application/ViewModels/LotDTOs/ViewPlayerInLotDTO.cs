using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.LotDTOs
{
    public class ViewPlayerInLotDTO
    {
        public int LotId { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public float? BidPrice { get; set; }
        public DateTime? BidTime { get; set; }

    }
}
