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
    public class JewelryRepository : GenericRepository<Jewelry>, IJewelryRepository
    {
        private readonly AppDbContext _dbContext;

        public JewelryRepository(
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
