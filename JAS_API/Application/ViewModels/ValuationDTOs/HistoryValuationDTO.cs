using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.ValuationDTOs
{
    public class HistoryValuationDTO
    {
        public string? StatusName { get; set; }
        public int? ValuationId { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
