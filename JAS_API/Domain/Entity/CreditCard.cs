using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class CreditCard : BaseEntity
    {
        public string? BankName { get; set; }
        public string? BankAccountHolder { get; set; }
        public string? BankCode { get; set; }
        public int? CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
    }
}
