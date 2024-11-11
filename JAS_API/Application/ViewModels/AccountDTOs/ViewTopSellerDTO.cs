using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.AccountDTOs
{
    public class ViewTopSellerDTO
    {
       public CustomerDTO customerDTO {  get; set; }
       public int TotalSellerValuation { get; set; }
    }

    public class ViewTopBuyerDTO
    {
        public CustomerDTO customerDTO { get; set; }
        public int TotalBuyerJewelry { get; set; }
    }
}
