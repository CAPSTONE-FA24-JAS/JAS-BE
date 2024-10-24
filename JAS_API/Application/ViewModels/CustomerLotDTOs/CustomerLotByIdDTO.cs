using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.CustomerLotDTOs
{
    public class CustomerLotByIdDTO
    {
        
        public int CustomerId { get; set; }
        public int LotId { get; set; }
        public float? PriceLimit { get; set; }


    }
}
