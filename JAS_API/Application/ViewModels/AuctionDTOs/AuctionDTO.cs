using Application.ViewModels.LotDTOs;
using AutoMapper;
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

        public virtual IEnumerable<LotDetailDTO>? LotDTOs { get; private set; } = Enumerable.Empty<LotDetailDTO>();

        public void SetLotDTOs(IEnumerable<Lot> lots, IMapper mapper)
        {
            LotDTOs = mapper.Map<IEnumerable<LotDetailDTO>>(lots);
        }
    }
}
