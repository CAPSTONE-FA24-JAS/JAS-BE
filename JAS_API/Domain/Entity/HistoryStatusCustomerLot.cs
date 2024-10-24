using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class HistoryStatusCustomerLot : BaseEntity
    {
        public DateTime? CurrentTime { get; set; }
        public string? Status { get; set; }
        public int? CustomerLotId { get; set; }

        //
        public virtual CustomerLot? CustomerLot { get; set; }
    }
}
