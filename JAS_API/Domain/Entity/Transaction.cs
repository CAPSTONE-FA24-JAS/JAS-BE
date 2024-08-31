using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Transaction : BaseEntity
    {
        public float? Amount { get; set; }
        public DateTime? TransactionTime { get; set; }
        public string? Status { get; set; }
        
        public int? WalletId { get; set; }
        public int? TransactionTypeId { get; set; }

        public virtual Wallet Wallet { get; set; }
        public virtual TransactionType TransactionType { get; set; }
    }
}
