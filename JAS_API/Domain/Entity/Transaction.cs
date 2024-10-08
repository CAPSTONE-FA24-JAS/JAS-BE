using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Transaction : BaseEntity
    {
        public int? DocNo { get; set; }
        public float? Amount { get; set; }
        public DateTime? TransactionTime { get; set; }
        public string? TransactionType { get; set; }
        public int? TransactionPerson { get; set; }
    }
}
