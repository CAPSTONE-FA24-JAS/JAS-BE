using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class TransactionType : BaseEntity
    {
        public string? Name { get; set; }
        public virtual IEnumerable<Transaction> Transactions { get; set;}
    }
}
