using Application.Interfaces;
using Application.Repositories;
using Domain.Entity;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories
{
    public class HistoryValuationRepository : GenericRepository<HistoryValuation>, IHistoryValuationRepository
    {
        private readonly AppDbContext _dbContext;
        public HistoryValuationRepository(
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
