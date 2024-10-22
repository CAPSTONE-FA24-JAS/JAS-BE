using Application.Interfaces;
using Application.Repositories;
using Domain.Entity;
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

        public async Task<(IEnumerable<Lot> data, int totalItems)> GetBidsOfCustomer(int? customerIId, string? status, int? pageIndex, int? pageSize)
        {
            var customerLots = _dbContext.Lots.Include(x => x.CustomerLots)
                                               .Where(x => x.CustomerLots.Any(cs => cs.CustomerId == customerIId && cs.Status == status));
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10; // Assuming a default pageSize of 10 if an invalid value is passed

                customerLots = customerLots.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            var products = await customerLots.ToListAsync();

            var totalItems = products.Count;

            if (products != null && products.Any())
            {
                return (products, totalItems);
            }
            else
            {
                throw new Exception("Don't have any Lots");
            }
        }


        public async Task<(IEnumerable<Lot> data, int totalItems)> GetPastBidOfCustomer(int customerIId, IEnumerable<string> status, int? pageIndex, int? pageSize)
        {
            var lots = _dbContext.Lots.Include(x => x.CustomerLots)
                                      .Where(x => x.CustomerLots.Any(cs => status.Contains(cs.Status) && cs.CustomerId == customerIId));

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10; // Assuming a default pageSize of 10 if an invalid value is passed

                lots = lots.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            var products = await lots.ToListAsync();

            var totalItems = products.Count;

            if (products != null && products.Any())
            {
                return (products, totalItems);
            }
            else
            {
                throw new Exception("Don't have any Lots");
            }


        }
    }
}
