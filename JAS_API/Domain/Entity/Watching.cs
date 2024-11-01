using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Watching : BaseEntity
    {
        public int? CustomerId { get; set; }
        public int? JewelryId { get; set; }
        //
        public virtual Customer? Customer { get; set; }
        public virtual Jewelry? Jewelry { get; set; }
    }
}
