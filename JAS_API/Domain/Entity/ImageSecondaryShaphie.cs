using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class ImageSecondaryShaphie : BaseEntity
    {
        public string? ImageLink { get; set; }
        public int? ShaphieId { get; set; }
        //
        public virtual SecondaryShaphie? SecondaryShaphie { get; set; }
    }
}
