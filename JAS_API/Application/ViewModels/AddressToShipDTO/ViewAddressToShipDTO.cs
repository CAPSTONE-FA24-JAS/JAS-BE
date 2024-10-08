
using Domain.Entity;

namespace Application.ViewModels.AddressToShipDTO
{
    public class ViewAddressToShipDTO
    {
        public string? AddressLine { get; set; }
        public int? CustomerId { get; set; }
        public int? WardId { get; set; }
        public string? WardName { get; set; }
        public string? DistrictName { get; set; }
        public string? ProvinceName { get; set; }

    }
}
