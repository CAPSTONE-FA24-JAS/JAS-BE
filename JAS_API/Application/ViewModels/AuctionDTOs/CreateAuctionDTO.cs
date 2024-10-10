namespace Application.ViewModels.AuctionDTOs
{
    public class CreateAuctionDTO
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
        //public string? Status { get; set; }
    }
}
