using Application.Interfaces;
using Application.Repositories;
using Domain.Entity;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories
{
    public class LotRepository : GenericRepository<Lot>, ILotRepository
    {
        private readonly AppDbContext _dbContext;
        public LotRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        )
            : base(context, timeService, claimsService)
        {
            _dbContext = context;
        }

        public List<Lot> GetLotsAsync(string lotType,string status)
        {
            var maxBidPrice = _dbContext.Lots.Where(x => x.LotType == lotType && x.Status == status).ToList();
            if (!maxBidPrice.Any())
            {
                return [];
            }
            else
            {
                return maxBidPrice;
            }
        }

        public async Task<List<Lot>> GetLotsForAutoServiceAsync(string lotType, string status)
        {
            var lots = await _dbContext.Lots.AsNoTracking().Where(x => x.LotType == lotType && x.Status == status)
                                                           .Select(x => new Lot
                                                           {  Id = x.Id,
                                                              Status = x.Status
                                                           })
                                                           .ToListAsync();
            if (!lots.Any())
            {
                return [];
            }
            else
            {
                return lots;
            }
        }



    }
}
