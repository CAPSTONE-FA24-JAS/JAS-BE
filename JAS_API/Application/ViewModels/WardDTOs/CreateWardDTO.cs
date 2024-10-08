using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.WardDTOs
{
    public class CreateWardDTO
    {
        public string? Name { get; set; }
        public int? DistrictId { get; set; }
    }
}
