using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class ImageJewelry : BaseEntity
    {
        public string? ImageLink { get; set; }
        public string? Title { get; set; }
        public string? ThumbnailImage { get; set; }
        public int? JewelryId { get; set; }
        //
        public virtual Jewelry? Jewelry { get; set; }
    }
}
