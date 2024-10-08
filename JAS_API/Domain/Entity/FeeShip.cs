using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class FeeShip : BaseEntity
    {
        public float? From { get; set; }
        public float? To { get; set; }
        public float? Free { get; set; }
    }
}
