

using Application.Interfaces;
using Application.Repositories;
using Domain.Entity;

namespace Infrastructures.Repositories
{
    public class FeeShipRepository : GenericRepository<FeeShip>, IFeeShipRepository
    {
        public FeeShipRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
        }
    }
}
