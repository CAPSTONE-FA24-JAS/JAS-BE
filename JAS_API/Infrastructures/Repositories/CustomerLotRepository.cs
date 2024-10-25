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
    public class CustomerLotRepository : GenericRepository<CustomerLot>, ICustomerLotRepository
    {
        private readonly AppDbContext _dbContext;
        public CustomerLotRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        )
            : base(context, timeService, claimsService)
        {
            _dbContext = context;
        }

        
       public async Task<CustomerLot> GetCustomerLotByCustomerAndLot(int? customerIId, int? lotId)
        {
            var customerLot = await _dbContext.CustomerLots.Where(x => x.CustomerId == customerIId && x.LotId == lotId).FirstOrDefaultAsync();
            if (customerLot == null)
            {
                throw new Exception("Not found CustomerLot");
            }
            return customerLot;
        }

        //get những thằng thua
        public List<CustomerLot>? GetListCustomerLotByCustomerAndLot(List<BidPrice> bidPriceList, int customerLotWinnerId)
        {
            var customerLots = _dbContext.CustomerLots
       .AsEnumerable() // Switch to client-side evaluation
       .Where(x => bidPriceList.Any(bd => bd.CustomerId == x.CustomerId && bd.LotId == x.LotId) && x.Id != customerLotWinnerId)
       .ToList();
            if (!customerLots.Any())
            {
                return null;
            }
            return customerLots;
        }

        public async Task<(IEnumerable<CustomerLot> data, int totalItems)> GetBidsOfCustomer(int? customerIId, string? status, int? pageIndex, int? pageSize)
        {
            var customerLots = _dbContext.CustomerLots.Include(x => x.Lot)
                                               .Where(x => x.CustomerId == customerIId && x.Status == status);
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


        public async Task<(IEnumerable<CustomerLot> data, int totalItems)> GetPastBidOfCustomer(int customerIId, IEnumerable<string> status, int? pageIndex, int? pageSize)
        {
            var lots = _dbContext.CustomerLots.Include(x => x.Lot)
                                      .Where(x =>  status.Contains(x.Status) && x.CustomerId == customerIId);

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
