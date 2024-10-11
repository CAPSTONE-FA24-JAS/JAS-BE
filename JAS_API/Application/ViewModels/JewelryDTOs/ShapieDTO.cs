using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.JewelryDTOs
{
    public class ShapieDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Color { get; set; }
        public float? Carat { get; set; }
        public string? EnhancementType { get; set; }
        public int? Quantity { get; set; }
        public string? SettingType { get; set; }
        public string? Dimension { get; set; }
        public int? JewelryId { get; set; }

        //
        public IEnumerable<DocumentDiamondDTO>? DocumentShaphies { get; set; }
        public IEnumerable<ImageDiamondDTO>? ImageShaphies { get; set; }
    }
}
