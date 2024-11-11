using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.LotDTOs
{
    public class CheckCustomerInLotDTO
    {
        public int CustomerId { get; set; }
        public int LotId { get; set; }
        public int CustomerLotId { get; set; }
        public bool Result { get; set; }
    }
    public class RequestCheckCustomerInLotDTO
    {
        public int CustomerId { get; set; }
        public int LotId { get; set; }
    }
}
