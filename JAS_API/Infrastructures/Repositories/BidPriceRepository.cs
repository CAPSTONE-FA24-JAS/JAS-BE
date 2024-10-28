using Application.Interfaces;
using Application.Repositories;
using Domain.Entity;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories
{
    public class BidPriceRepository : GenericRepository<BidPrice>, IBidPriceRepository
    {
        private readonly AppDbContext _dbContext;
        public BidPriceRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        )
            : base(context, timeService, claimsService)
        {
            _dbContext = context;
        }


       public async Task<BidPrice> GetMaxBidPriceByCustomerIdAndLot(int? customerId, int? lotId)
        {
            var maxBidPrice = await _dbContext.BidPrices.Where( x => x.CustomerId == customerId  && x.LotId == lotId).OrderByDescending(x => x.CurrentPrice).FirstOrDefaultAsync();
            if(maxBidPrice == null)
            {
                return null;
            }
            else
            {
                return maxBidPrice;
            }
        }


        //cho hinh thuc 3
        public List<BidPrice> GetMaxBidPriceByLotId(int lotId)
        {
            var maxBidPrice = _dbContext.BidPrices.Where(x => x.LotId == lotId).ToList();
            if (!maxBidPrice.Any())
            {
                return [];
            }
            else
            {
                return maxBidPrice;
            }
        }

        //cho hinh thuc 4 dau gia ngc, chac chan chi cos 1 bidPrice
        public async Task<BidPrice> GetBidPriceByLotIdForReduceBidding(int lotId)
        {
            var maxBidPrice = await _dbContext.BidPrices.Where(x => x.LotId == lotId).FirstOrDefaultAsync();
            if (maxBidPrice == null)
            {
                return null;
            }
            else
            {
                return maxBidPrice;
            }
        }

    }
}
