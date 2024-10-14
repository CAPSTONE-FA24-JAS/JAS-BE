using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class ImageMainDiamond : BaseEntity
    {
        public string? ImageLink { get; set; }
        public int? MainDiamondId { get; set; }

        //
        public virtual MainDiamond? MainDiamond { get; set; }
    }
}
