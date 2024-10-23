using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.WalletDTOs
{
    public class RequestWithdrawDTO
    {
        public int WalletId { get; set; }
        public float Amount { get; set; }
    }
}
