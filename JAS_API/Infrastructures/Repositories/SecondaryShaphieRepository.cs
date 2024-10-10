using Application.Interfaces;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories
{
    public class SecondaryShaphieRepository : GenericRepository<SecondaryShaphie>, ISecondaryShaphieRepository
    {
        private readonly AppDbContext _dbContext;
        public SecondaryShaphieRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        )
            : base(context, timeService, claimsService)
        {
            _dbContext = context;
        }
    }
}
