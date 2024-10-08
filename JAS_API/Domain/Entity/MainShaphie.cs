using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class MainShaphie : BaseEntity
    {
        public string? Color { get; set; }
        public float? Carat { get; set; }
        public string? EnhancementType { get; set; }
        public int? Quantity { get; set; }
        public string? SettingType { get; set; }
        public float? Dimension { get; set; }
        public int? JewelryId { get; set; }

        //
        public virtual IEnumerable<DocumentMainShaphie>? DocumentMainShaphies { get; set; }
        public virtual IEnumerable<ImageMainShaphie>? ImageMainShaphies { get; set; }
        public virtual Jewelry? Jewelry { get; set; }
    }
}
