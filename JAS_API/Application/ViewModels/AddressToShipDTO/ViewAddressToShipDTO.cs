
using Domain.Entity;

namespace Application.ViewModels.AddressToShipDTO
{
    public class ViewAddressToShipDTO
    {
        public int Id { get; set; }
        public string? AddressLine { get; set; }
        public int? CustomerId { get; set; }
        public bool? IsDefault { get; set; }
    }
}
