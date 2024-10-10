using Application.Interfaces;
using Application.Repositories;
using Domain.Entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories
{
    public class MainDiamondRepository : GenericRepository<MainDiamond>, IMainDiamondRepository
    {
        private readonly AppDbContext _dbContext;
        public MainDiamondRepository(
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
