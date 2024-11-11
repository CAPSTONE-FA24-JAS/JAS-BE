using Application.Interfaces;
using Application.Repositories;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories
{
    public class AuctionRepository : GenericRepository<Auction>, IAuctionRepository
    {
        private readonly AppDbContext _dbContext;
        public AuctionRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        )
            : base(context, timeService, claimsService)
        {
            _dbContext = context;
        }


        public List<Auction> GetAuctionsAsync(string status)
        {
            var maxBidPrice = _dbContext.Auctions.Where(x =>  x.Status == status).ToList();
            if (!maxBidPrice.Any())
            {
                return [];
            }
            else
            {
                return maxBidPrice;
            }
        }
    }
}
