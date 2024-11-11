using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class HistoryValuation : BaseEntity
    {
        public string? StatusName { get; set; }
        public int? ValuationId { get; set; }
        //
        public virtual Valuation? Valuation { get; set; }
    }
}
