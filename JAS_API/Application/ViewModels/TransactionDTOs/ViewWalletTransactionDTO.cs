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
        public string? transactionType { get; set; }
        public float? Amount { get; set; }
        public DateTime? TransactionTime { get; set; }
        public string? Status { get; set; }
        
    }

    public class ViewTransactionDTO
    {
        public string? transactionType { get; set; }
        public float? Amount { get; set; }
        public DateTime? TransactionTime { get; set; }
        public string? CustomerName { get; set; }

    }
}
