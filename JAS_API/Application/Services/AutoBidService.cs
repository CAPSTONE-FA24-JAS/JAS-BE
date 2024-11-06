using Application.Interfaces;
using Application.ServiceReponse;
using Application.ViewModels.AutoBidDTOs;
using Application.ViewModels.CategoryDTOs;
using AutoMapper;
using Domain.Entity;

namespace Application.Services
{
    public class AutoBidService : IAutoBidService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AutoBidService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<APIResponseModel> SetAutoBid(CreateAutoBidDTO createAutoBidDTO)
        {
            var response = new APIResponseModel();
            try
            {

                var autoBid = _mapper.Map<AutoBid>(createAutoBidDTO);
                autoBid.IsActive = true;
                var customerLotOfAutoBid = await _unitOfWork.CustomerLotRepository.GetByIdAsync(createAutoBidDTO.CustomerLotId);
                customerLotOfAutoBid.IsAutoBid = true;
                if (autoBid == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Mapper failed";
                }
                else
                {
                    _unitOfWork.AutoBidRepository.AddAsync(autoBid);
                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        response.Message = $"Set New AutoBid Successfully";
                        response.Code = 200;
                        response.IsSuccess = true;
                    }
                    else
                    {
                        response.Message = $"Save DB failed!";
                        response.Code = 400;
                        response.IsSuccess = false;
                    }
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
