using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.CreditCardDTOs
{
    public class CreateCreditCardDTO
    {
        public string? BankName { get; set; }
        public string? BankAccountHolder { get; set; }
        public string? BankCode { get; set; }
        public int? CustomerId { get; set; }
    }
}
