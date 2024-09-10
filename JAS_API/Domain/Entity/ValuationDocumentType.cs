using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class ValuationDocumentType : BaseEntity
    {
        public string? Name { get; set; }
        //
        public virtual IEnumerable<ValuationDocument>? ValuationDocuments { get; set; }
    }
}
