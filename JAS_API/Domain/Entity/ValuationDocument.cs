﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class ValuationDocument : BaseEntity
    {
        public string? DocumentLink { get; set; }
        public int? ValuationId { get; set; }
        public string? ValuationDocumentType { get; set; }
        public string? SignatureCode { get; set; }
        //
        public virtual Valuation? Valuation { get; set; }
    }
}
