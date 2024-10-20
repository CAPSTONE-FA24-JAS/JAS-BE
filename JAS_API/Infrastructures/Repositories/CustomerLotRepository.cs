using Application.Interfaces;
using Application.Repositories;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

     
    }
}
