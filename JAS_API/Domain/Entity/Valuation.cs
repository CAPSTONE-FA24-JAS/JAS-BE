using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Valuation : BaseEntity
    {
        public string? Name { get; set; }
        public DateTime? PricingTime { get; set; }
        public float? DesiredPrice { get; set; }
        public float? Height { get; set; }
        public float? Width { get; set; }
        public float? Depth { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? ImageOfReceip { get; set;}
        public string? ActualStatusOfJewelry { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int? Quantity { get; set; }
        public int? SellerId { get; set; }
        public int? StaffId { get; set; }

        //Enity Relationship
        public virtual Account? Seller { get; set; }
        public virtual Account? Staff { get; set; }
        public virtual IEnumerable<ImageValuation>? ImageValuations { get; set; }
        public virtual IEnumerable<ValuationDocument>? ValuationDocuments { get; set; }
    }
}
