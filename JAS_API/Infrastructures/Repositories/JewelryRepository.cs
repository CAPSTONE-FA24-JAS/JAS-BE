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


      public async Task<(IEnumerable<Jewelry> data, int totalItem)> GetAllJewelryAynsc(int? pageIndex = null, int? pageSize = null)
        {
            var jewelry = _dbContext.Jewelries.Include(x => x.Artist)
                                              .Include(x => x.Category)
                                              .Include(x => x.ImageJewelries)
                                              .Include(x => x.KeyCharacteristicDetails)
                                                      .ThenInclude(kc => kc.KeyCharacteristic)
                                              .Include(x => x.Lot)
                                              .Include(x => x.MainDiamonds)
                                                      .ThenInclude(md => md.ImageMainDiamonds)
                                              .Include(x => x.SecondaryDiamonds)
                                                      .ThenInclude(sd => sd.ImageSecondaryDiamonds)
                                              .Include(x => x.MainShaphies)
                                                      .ThenInclude(ms => ms.ImageMainShaphies)
                                              .Include(x => x.SecondaryShaphies)
                                                      .ThenInclude(ss => ss.ImageSecondaryShaphies)
                                              .Include(x => x.Valuation)
                                              .Where(x => x.IsDeleted == false);

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10; // Assuming a default pageSize of 10 if an invalid value is passed

                jewelry = jewelry.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            var products = await jewelry.ToListAsync();
            
            var totalItems = products.Count;

            if (products != null && products.Any())
            {
                return (products, totalItems);
            }
            else
            {
                throw new Exception("Don't have any Product");
            }


        }


        public async Task<(IEnumerable<Jewelry> data, int totalItem)> GetAllJewelryNoLotAynsc(int? pageIndex = null, int? pageSize = null)
        {
            var jewelry = _dbContext.Jewelries.Include(x => x.Artist)
                                              .Include(x => x.Category)
                                              .Include(x => x.ImageJewelries)
                                              .Include(x => x.KeyCharacteristicDetails)
                                                      .ThenInclude(kc => kc.KeyCharacteristic)
                                              .Include(x => x.Lot)
                                              .Include(x => x.MainDiamonds)
                                                      .ThenInclude(md => md.ImageMainDiamonds)
                                              .Include(x => x.SecondaryDiamonds)
                                                      .ThenInclude(sd => sd.ImageSecondaryDiamonds)
                                              .Include(x => x.MainShaphies)
                                                      .ThenInclude(ms => ms.ImageMainShaphies)
                                              .Include(x => x.SecondaryShaphies)
                                                      .ThenInclude(ss => ss.ImageSecondaryShaphies)
                                              .Include(x => x.Valuation)
                                              .Where(x => x.IsDeleted == false && x.Status == null);

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10; // Assuming a default pageSize of 10 if an invalid value is passed

                jewelry = jewelry.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            var products = await jewelry.ToListAsync();

            var totalItems = products.Count;

            if (products != null && products.Any())
            {
                return (products, totalItems);
            }
            else
            {
                throw new Exception("Don't have any Product");
            }


        }
    }
}
