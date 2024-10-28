using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.TransactionDTOs
{
    public class ViewWalletTransactionDTO 
    {
        //public string? transactionId { get; set; }
        public string? transactionType { get; set; }
        //public int? DocNo { get; set; }
        public float? Amount { get; set; }
        public DateTime? TransactionTime { get; set; }
        //public int transactionPerson { get; set; }
        public string? Status { get; set; }
        //public int? WalletId { get; set; }
        
    }
}
