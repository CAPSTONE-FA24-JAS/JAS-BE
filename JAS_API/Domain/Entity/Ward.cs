using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Ward : BaseEntity
    {
        public string? Name { get; set; }
        public int? DistrictId { get; set; }

        //
        public virtual District District { get; set; }
        //public virtual IEnumerable<AddressToShip> AddressToShips { get; set; }
    }
}
