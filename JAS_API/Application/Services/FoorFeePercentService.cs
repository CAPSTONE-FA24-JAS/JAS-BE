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
                                                                                              && x.To >= currentPrice);

            if(percentCurrent.Count > 0)
            {
                return percentCurrent.SingleOrDefault()?.Percent;
            }
            return null;
        }
    }
}
