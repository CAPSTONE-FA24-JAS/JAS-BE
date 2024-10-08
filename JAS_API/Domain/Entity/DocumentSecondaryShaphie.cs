using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class DocumentSecondaryShaphie : BaseEntity
    {
        public string? DocumentLink { get; set; }
        public string? DocumentTitle { get; set; }
        public int? ShaphieId { get; set; }
        //
        public virtual SecondaryShaphie? SecondaryShaphie { get; set; }
    }
}
