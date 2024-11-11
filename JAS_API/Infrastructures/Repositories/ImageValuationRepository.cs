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
    public class ImageValuationRepository : GenericRepository<ImageValuation>, IImageValuationRepository
    {
        private readonly AppDbContext _dbContext;

        public ImageValuationRepository(
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
