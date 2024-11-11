using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.AuctionDTOs
{
    public class AuctionDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public string? Description { get; set; }
        public string? ImageLink { get; set; }
        public string? Status { get; set; }
        public int? TotalLot {  get; set; } 

        //public virtual IEnumerable<Lot>? Lots { get; set; }
    }
}
