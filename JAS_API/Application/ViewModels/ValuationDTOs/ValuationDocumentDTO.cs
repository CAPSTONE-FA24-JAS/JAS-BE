using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.ValuationDTOs
{
    public class ValuationDocumentDTO
    {
        public int Id { get; set; }
        public string? DocumentLink { get; set; }
        public int? ValuationId { get; set; }
        public string? ValuationDocumentType { get; set; }
        public string? SignatureCode { get; set; }

        public DateTime CreationDate { get; set; }

        public int? CreatedBy { get; set; }
        //
    }
}
