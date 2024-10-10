using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class SecondaryShaphie : BaseEntity
    {
        public string? Name { get; set; }
        public string? Color { get; set; }
        public float? Carat { get; set; }
        public string? EnhancementType { get; set; }
        public int? Quantity { get; set; }
        public string? SettingType { get; set; }
        public string? Dimension { get; set; }
        public float? TotalCarat { get; set; }
        public int? JewelryId { get; set; }

        //
        public virtual IEnumerable<DocumentSecondaryShaphie>? DocumentSecondaryShaphies { get; set; }
        public virtual IEnumerable<ImageSecondaryShaphie>? ImageSecondaryShaphies { get; set; }
        public virtual Jewelry? Jewelry { get; set; }
    }
}
