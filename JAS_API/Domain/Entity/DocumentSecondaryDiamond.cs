using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class DocumentSecondaryDiamond : BaseEntity
    {
        public string? DocumentLink { get; set; }
        public string? DocumentTitle { get; set; }
        public int? SecondaryDiamondId { get; set; }
        //
        public virtual SecondaryDiamond? SecondaryDiamond { get; set; }
    }
}
