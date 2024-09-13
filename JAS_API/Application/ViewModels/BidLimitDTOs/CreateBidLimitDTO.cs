using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.BidLimitDTOs
{
    public class CreateBidLimitDTO
    {
        public IFormFile? File { get; set; }
        public float? PriceLimit { get; set; }
        public int? AccountId { get; set; }
    }
}
