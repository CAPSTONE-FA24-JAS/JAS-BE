using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services
{
    public class FoorFeePercentService : IFoorFeePercentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FoorFeePercentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<float?> GetPercentFloorFeeOfLot(float currentPrice)
        {
            var percentCurrent = await _unitOfWork.FloorFeePersentRepository.GetAllAsync(x => x.From <= currentPrice
                                                                                              && (x.To == null || x.To >= currentPrice));
            var bestMatch = percentCurrent.OrderBy(x => x.To ?? float.MaxValue).FirstOrDefault();
            if (bestMatch != null)
            {
                return bestMatch?.Percent;
            }
            return null;
        }
    }
}
