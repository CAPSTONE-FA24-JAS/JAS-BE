using Microsoft.AspNetCore.Http;

namespace Application.ViewModels.AuctionDTOs
{
    public class CreateAuctionDTO
    {
        public string? Name { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Description { get; set; }
        public IFormFile? FileImage { get; set; }
        //public string? Status { get; set; }
    }
}
