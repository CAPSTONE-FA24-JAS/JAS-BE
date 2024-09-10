using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class ImageValuation : BaseEntity
    {
        public string? ImageLink { get; set; }
        public int? ValuationId { get; set; }

        //RelationShip
        public virtual Valuation? Valuation { get; set; }

    }
}
