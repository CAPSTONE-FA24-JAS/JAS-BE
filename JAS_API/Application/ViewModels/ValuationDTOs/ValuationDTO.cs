
using Application.ViewModels.AccountDTOs;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.ValuationDTOs
{
    public class ValuationDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? PricingTime { get; set; }
        public float? Height { get; set; }
        public float? Width { get; set; }
        public float? Depth { get; set; }
        public float? EstimatePriceMin { get; set; }
        public float? EstimatePriceMax { get; set; }
        public string? ImageOfReceip { get; set; }
        public string? ActualStatusOfJewelry { get; set; }
        public string? Status { get; set; }
        public string? CancelReason { get; set; }
        public int? SellerId { get; set; }
        public int? StaffId { get; set; }
        public DateTime CreationDate { get; set; }
        public CustomerDTO? Seller { get; set; }

        public StaffDTO? Staff { get; set; }
        public IEnumerable<ImageValuationDTO>? ImageValuations { get; set; }
        public IEnumerable<ValuationDocumentDTO>? ValuationDocuments { get; set; }
    }
}
