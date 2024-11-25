using Application.ViewModels.CreditCardDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.WalletDTOs
{
    public class RequestWithdrawDTO
    {
        public int CustomerId { get; set; }
        public int WalletId { get; set; }
        public float Amount { get; set; }
    }
    public class ViewRequestWithdrawDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int WalletId { get; set; }
        public string Status { get; set; }
        public float Amount { get; set; }
        public ViewCreditCardDTO? ViewCreditCardDTO { get; set; }
    }

}
