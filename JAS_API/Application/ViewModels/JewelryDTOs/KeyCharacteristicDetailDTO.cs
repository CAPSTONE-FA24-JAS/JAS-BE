using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.JewelryDTOs
{
    public class KeyCharacteristicDetailDTO
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public int? JewelryId { get; set; }
        public int? KeyCharacteristicId { get; set; }

    }
}
