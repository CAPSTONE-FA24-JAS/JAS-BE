using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.FloorFeeDTOs
{
    public class UpdateFloorFeeDTO
    {
        public int Id { get; set; }
        public float? From { get; set; }
        public float? To { get; set; }
        public float? Percent { get; set; }
    }
}
