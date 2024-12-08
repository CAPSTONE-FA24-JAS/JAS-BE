using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.ValuationDTOs
{
    public class ImageValuationDTO
    {
        public int Id { get; set; }
        public string? ImageLink { get; set; }
        public int? ValuationId { get; set; }

        public string? DefaultImage { get; set; }
    }
}
