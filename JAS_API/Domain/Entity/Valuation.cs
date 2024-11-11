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
        public string? Description { get; set; }
        public DateTime? PricingTime { get; set; }
        public float? Height { get; set; }
        public float? Width { get; set; }
        public float? Depth { get; set; }
        public float? EstimatePriceMin { get; set; }
        public float? EstimatePriceMax { get; set; }
        public float? SpecificPrice { get; set; }
        public string? ImageOfReceip { get; set;}
        public string? ActualStatusOfJewelry { get; set; }
        public string? Status { get; set; }
        public string? CancelReason { get; set; }
        public int? SellerId { get; set; }
        public int? StaffId { get; set; }
        public int? AppraiserId { get; set; }

        //Enity Relationship
        public virtual Customer? Seller { get; set; }
        public virtual Staff? Staff { get; set; }
        public virtual Staff? Appraiser { get; set; }
        public virtual IEnumerable<ImageValuation>? ImageValuations { get; set; }
        public virtual IEnumerable<ValuationDocument>? ValuationDocuments { get; set; }
        public virtual IEnumerable<HistoryValuation>? HistoryValuations { get; set; }
        public virtual Jewelry? Jewelry { get; set; }
    }
}
