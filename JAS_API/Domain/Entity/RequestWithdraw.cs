using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class RequestWithdraw : BaseEntity
    {
        public float? Amount { get; set; }
        public int? WalletId { get; set; }
        //
        public virtual Wallet? Wallet { get; set; }
        //public  WalletTransaction? RequestOfWalletTransaction { get; set; }
    }
}
