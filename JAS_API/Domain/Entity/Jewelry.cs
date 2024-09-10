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
        public float? FinalPrice { get; set; }
        public float? ReserverPrice { get; set; }
        public string? Condition { get; set; }
        public string? FileVideo { get; set; }
        public int? ArtistId { get; set; }
        public int? GenderId { get; set; }
        public int? CategoryId { get; set; }

        //
        public virtual Artist? Artist { get; set; }
        public virtual Gender? Gender { get; set; }
        public virtual Category? Category { get; set;}
        public virtual Proof? Proof { get; set; }
        public virtual IEnumerable<ImageJewelry>? ImageJewelries { get; set; }
        public virtual IEnumerable<KeyCharacteristicDetail>? KeyCharacteristicDetails { get; set; }
    }
}
