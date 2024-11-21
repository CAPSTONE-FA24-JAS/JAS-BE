using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.CustomerLotDTOs;
using Domain.Enums;
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

        public async Task<APIResponseModel> GetFloorFeesAsync()
        {
            var response = new APIResponseModel();
            try
            {
                
                var floorFees = await _unitOfWork.FloorFeePersentRepository.GetAllAsync();

                
                if (floorFees != null)
                {
                    response.Message = $"List customerLot Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = floorFees;
                }
                else
                {
                    response.Message = $"Don't have floorFees";
                    response.Code = 404;
                    response.IsSuccess = true;

                }
            }
            catch (Exception ex)
            {
                response.ErrorMessages = ex.Message.Split(',').ToList();
                response.Message = "Exception";
                response.Code = 500;
                response.IsSuccess = false;
            }
            return response;
        }

    }
}
