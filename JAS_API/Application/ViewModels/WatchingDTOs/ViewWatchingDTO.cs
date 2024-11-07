using Application.ViewModels.JewelryDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.WatchingDTOs
{
    public class ViewWatchingDTO
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? JewelryId { get; set; }
        public JewelryDTO? jewelryDTO { get; set; }
    }
}
