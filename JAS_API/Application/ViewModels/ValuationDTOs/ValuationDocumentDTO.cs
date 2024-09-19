﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.ValuationDTOs
{
    public class ValuationDocumentDTO
    {

        public string? FileDocument { get; set; }
        public int? ValuationId { get; set; }
        public int? ValuationDocumentTypeId { get; set; }

        public DateTime CreationDate { get; set; }

        public int? CreatedBy { get; set; }
        //
    }
}