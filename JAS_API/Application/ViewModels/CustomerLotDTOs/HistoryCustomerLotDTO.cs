using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.CustomerLotDTOs
{
    public class HistoryCustomerLotDTO
    {
        public DateTime? CurrentTime { get; set; }
        public string? Status { get; set; }
        public int? CustomerLotId { get; set; }

    }
}
