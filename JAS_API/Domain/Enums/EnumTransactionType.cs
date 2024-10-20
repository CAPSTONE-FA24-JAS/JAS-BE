using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum  EnumTransactionType
    {
        AddWallet = 1,
        WithDrawWallet = 2,
        DepositWallet = 3,
        RefundDeposit = 4,
        BuyPay = 5,
        SellerPay = 6
    }
}
