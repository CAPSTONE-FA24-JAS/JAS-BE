using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class BidIncrement : BaseEntity
    {
        public float? From { get; set; }
        public float? To { get; set; }
        public float? PricePerStep { get; set; }
    }
}
