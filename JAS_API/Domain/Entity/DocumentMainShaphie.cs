using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class DocumentMainShaphie : BaseEntity
    {
        public string? DocumentLink { get; set; }
        public string? DocumentTitle { get; set; }
        public int? ShaphieId { get; set; }
        //
        public virtual MainShaphie? MainShaphie { get; set; }
    }
}
