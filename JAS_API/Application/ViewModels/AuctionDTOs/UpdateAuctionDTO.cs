using Microsoft.AspNetCore.Http;

namespace Application.ViewModels.AuctionDTOs
{
    public class UpdateAuctionDTO
    {
        public int AutionId { get; set; }
        public string? Name { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Description { get; set; }
        public IFormFile? FileImage { get; set; }
    }
}
