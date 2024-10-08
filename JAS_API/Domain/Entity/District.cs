using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class District : BaseEntity
    {
        public string? Name { get; set; }
        public int? ProvinceId { get; set; }
        //

        public virtual Province? Province { get; set; }
        public virtual IEnumerable<Ward>? Wards { get; set; }
    }
}
