using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class DocumentMainDiamond : BaseEntity
    {
        public string? DocumentLink { get; set; }
        public string? DocumentTitle { get; set; }
        public int? DiamondId { get; set; }
        //
        public virtual MainDiamond? MainDiamond { get; set; }
    }
}
