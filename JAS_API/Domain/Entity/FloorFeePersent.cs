using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class FloorFeePersent : BaseEntity
    {
        public float? From { get; set; }
        public float? To { get; set; }
        public float? Percent { get; set; }
    }
}
