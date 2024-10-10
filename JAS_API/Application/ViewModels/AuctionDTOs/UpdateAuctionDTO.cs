using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.AuctionDTOs
{
    public class UpdateAuctionDTO
    {
        public int AutionId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
    }
}
