using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Jewelry : BaseEntity 
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public float? EstimatePriceMin { get; set; }
        public float? EstimatePriceMax { get; set; }
        public float? StartingPrice { get; set; }
        public string? VideoLink { get; set; }
        public string? ForGender { get; set; }
        public string? Title { get; set; }
        public string? BidForm { get; set; }
        public DateTime? Time_Bidding { get; set; }
        public int? ArtistId { get; set; }
        public int? CategoryId { get; set; }
        public int? ValuationId { get; set; }

        //
        public virtual Artist? Artist { get; set; }
        public virtual Category? Category { get; set;}
        public virtual IEnumerable<ImageJewelry>? ImageJewelries { get; set; }
        public virtual IEnumerable<KeyCharacteristicDetail>? KeyCharacteristicDetails { get; set; }
        public virtual Lot? Lot { get; set; }
        public virtual IEnumerable<MainDiamond>? MainDiamonds { get; set; }
        public virtual IEnumerable<SecondaryDiamond>? SecondaryDiamonds { get; set;}
        public virtual IEnumerable<MainShaphie>? MainShaphies { get; set; }
        public virtual IEnumerable<SecondaryShaphie>? SecondaryShaphies { get; set; }
        public virtual Valuation? Valuation { get; set; }
    }
}
