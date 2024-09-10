using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class ValuationDocument : BaseEntity
    {
        public string? FileDocument { get; set; }
        public int? ValuationId { get; set; }
        public int? ValuationDocumentTypeId { get; set; }
        //
        public virtual Valuation? Valuation { get; set; }
        public virtual ValuationDocumentType? ValuationDocumentType { get; set;}
    }
}
