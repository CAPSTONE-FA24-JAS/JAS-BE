using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IBidPriceRepository : IGenericRepository<BidPrice>
    {
        Task<BidPrice> GetMaxBidPriceByCustomerIdAndLot(int? customerId, int? lotId);
        List<BidPrice> GetMaxBidPriceByLotId(int lotId);


        //cho hinh thuc 4 dau gia ngc, chac chan chi cos 1 bidPrice
        Task<BidPrice> GetBidPriceByLotIdForReduceBidding(int lotId);
    }
}
