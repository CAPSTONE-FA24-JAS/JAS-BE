using Application.Interfaces;
using Application.Repositories;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;



namespace Infrastructures.Repositories
{
    public class JewelryRepository : GenericRepository<Jewelry>, IJewelryRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;

        public JewelryRepository(
           AppDbContext context,
           ICurrentTime timeService,
           IClaimsService claimsService
       )
           : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }


      public async Task<IEnumerable<Jewelry>> GetAllAynsc(int? pageIndex = null, int? pageSize = null)
        {
            var jewelry = _dbContext.Jewelries.Include(x => x.Artist);
                                              
           

            

            var products = await jewelry.ToListAsync();

            if (products != null && products.Any())
            {
                return products;
            }
            else
            {
                throw new Exception("Don't have any Product");
            }


        }
    }
}
