using Application.ViewModels.LotDTOs;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.CustomerLotDTOs
{
    public class MyBidDetailDTO : MyBidDTO
    {
        public IEnumerable<HistoryCustomerLotDTO>? HistoryStatusCustomerLots { get; set; }

    }
}
