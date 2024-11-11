using Application.Interfaces;
using Application.Repositories;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructures.Repositories
{
    public class WalletRepository : GenericRepository<Wallet>, IWalletRepository
    {
        private readonly AppDbContext _dbContext;
        public WalletRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        )
            : base(context, timeService, claimsService)
        {
            _dbContext = context;
        }

        public async Task<Wallet> GetByCustomerId(int? customerId)
        {
            var wallet = await _dbContext.Wallets.Where(x => x.CustomerId == customerId).FirstOrDefaultAsync();
            if ( wallet == null )
            {
                throw new Exception("not found Wallet by customerId");
            }
            else
            {
                return wallet;
            }

        }
    }
}
