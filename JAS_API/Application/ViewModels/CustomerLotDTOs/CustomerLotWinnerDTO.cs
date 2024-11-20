using Application.ViewModels.AccountDTOs;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.CustomerLotDTOs
{
    public class CustomerLotWinnerDTO
    {
       
        public float? CurrentPrice { get; set; }
       
        public bool? IsDeposit { get; set; }
        public int? CustomerId { get; set; }
        public int? LotId { get; set; }
        
        public float? PriceLimit { get; set; }
        public bool? IsWinner { get; set; }
        public bool? IsRefunded { get; set; }
        public bool? IsInvoiced { get; set; }
        public DateTime? ExpireDateOfBidLimit { get; set; }

        public  SellerDTO? Customer { get; set; }
        
      
    }
}
