using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class ImageSecondaryDiamond : BaseEntity
    {
        public string? ImageLink { get; set; }
        public int? SecondaryDiamondId { get; set; }

        //
        public virtual SecondaryDiamond? SecondaryDiamond { get; set; }
    }
}
