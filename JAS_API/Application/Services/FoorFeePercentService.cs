using Application.Interfaces;
using Application.ServiceReponse;
using Application.ViewModels.FloorFeeDTOs;

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

        public async Task<APIResponseModel> UpdateFloorFeesAsync(UpdateFloorFeeDTO dto)
        {
            var response = new APIResponseModel();

            if (dto == null)
            {
                response.Message = "Input cannot be null.";
                response.Code = 404;
                response.IsSuccess = false;
                return response;
            }

            if (dto.Percent is not null && (dto.Percent < 0 || dto.Percent > 100))
            {
                response.Message = "Percent must be between 0 and 100.";
                response.Code = 400;
                response.IsSuccess = false;
                return response;
            }

            try
            {
                var floorFee = await _unitOfWork.FloorFeePersentRepository.GetByIdAsync(dto.Id);
                if (floorFee == null)
                {
                    response.Message = "Floor fee not found.";
                    response.Code = 404;
                    response.IsSuccess = false;
                    return response;
                }

                var from = dto.From ?? floorFee.From;
                var to = dto.To ?? floorFee.To;

                if (from.HasValue && to.HasValue)
                {
                    var existingRanges = await _unitOfWork.FloorFeePersentRepository.GetAllAsync();

                    bool isOverlap = existingRanges.Any(ff =>
                        ff.Id != dto.Id && // Loại bỏ bản ghi hiện tại
                        ((from.Value >= ff.From && from.Value < ff.To) || // `from` mới nằm trong phạm vi cũ
                         (to.Value > ff.From && to.Value <= ff.To) ||    // `to` mới nằm trong phạm vi cũ
                         (from.Value <= ff.From && to.Value >= ff.To))  // Khoảng mới bao phủ toàn bộ khoảng cũ
                    );

                    if (isOverlap)
                    {
                        response.Message = "The specified range [From, To] overlaps with an existing range.";
                        response.Code = 400;
                        response.IsSuccess = false;
                        return response;
                    }
                }

                // Cập nhật thông tin
                floorFee.From = dto.From ?? floorFee.From;
                floorFee.To = dto.To ?? floorFee.To;
                floorFee.Percent = dto.Percent ?? floorFee.Percent;

                if (await _unitOfWork.SaveChangeAsync() > 0)
                {
                    response.Message = "Floor fee updated successfully.";
                    response.Code = 200;
                    response.IsSuccess = true;
                }
                else
                {
                    response.Message = "Failed to update floor fee.";
                    response.Code = 400;
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.Message = "An unexpected error occurred.";
                response.Code = 500;
                response.IsSuccess = false;
            }

            return response;
        }
    }
}
