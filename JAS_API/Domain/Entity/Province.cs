using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Province : BaseEntity
    {
        public string? Name { get; set; }

        //
        public virtual IEnumerable<District>? Districts { get; set; }
    }
}
