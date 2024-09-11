using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.ValuationDTOs
{
    public class ConsignAnItemDTO
    {
        public string? Name { get; set; }
        public float? Height { get; set; }
        public float? Width { get; set; }
        public float? Depth { get; set; }
        public string? Description { get; set; }
        public int? SellerId { get; set; }
        
        public List<IFormFile>? ImageValuation { get; set; }
    }
}
