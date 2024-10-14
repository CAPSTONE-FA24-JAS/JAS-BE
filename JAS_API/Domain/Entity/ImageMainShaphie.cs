using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class ImageMainShaphie : BaseEntity
    {
        public string? ImageLink { get; set; }
        public int? MainShaphieId { get; set; }
        //
        public virtual MainShaphie? MainShaphie { get; set; }
    }
}
