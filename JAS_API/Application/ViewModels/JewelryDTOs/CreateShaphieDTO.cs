using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.JewelryDTOs
{
    public class CreateShaphieDTO
    {
        public string? Color { get; set; }
        public float? Carat { get; set; }
        public string? EnhancementType { get; set; }
        public int? Quantity { get; set; }
        public string? SettingType { get; set; }
        public float? Dimension { get; set; }

        //
        public List<IFormFile>? DocumentShaphies { get; set; }
        public List<IFormFile>? ImageShaphies { get; set; }
    }
}
