using Application.Interfaces;
using Application.ServiceReponse;
using Application.ViewModels.AutoBidDTOs;
using Application.ViewModels.CategoryDTOs;
using AutoMapper;
using Domain.Entity;
using Domain.Enums;

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
                var customerLotOfAutoBid = await _unitOfWork.CustomerLotRepository.GetByIdAsync(createAutoBidDTO.CustomerLotId);
                if (customerLotOfAutoBid == null)
                {
                    return CreateErrorResponse("Customer lot not found", 404);
                }

                var lot = customerLotOfAutoBid.Lot;
                if (lot == null || lot.LotType != EnumLotType.Public_Auction.ToString())
                {
                    return CreateErrorResponse("Invalid lot type for AutoBid", 400);
                }

                bool isFinalPriceSold = lot.FinalPriceSold != null;

                if (IsvalidBidRange(customerLotOfAutoBid, createAutoBidDTO, isFinalPriceSold) == false)
                {
                    return CreateErrorResponse("Set New AutoBid Failed, Autobid of player must out range old autobid", 400);
                }

                var autoBid = _mapper.Map<AutoBid>(createAutoBidDTO);
                if (autoBid == null)
                {
                    return CreateErrorResponse("Mapper failed", 400);
                }

                autoBid.IsActive = true;
                customerLotOfAutoBid.IsAutoBid = true;

                await _unitOfWork.AutoBidRepository.AddAsync(autoBid);

                if (await _unitOfWork.SaveChangeAsync() > 0)
                {
                    response.Message = "Set New AutoBid Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                }
                else
                {
                    return CreateErrorResponse("Save DB failed!", 400);
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

        private bool IsvalidBidRange(CustomerLot customerLot, CreateAutoBidDTO createAutoBidDTO, bool isFinalPriceSold)
        {
            var latestAutoBid = customerLot.AutoBids
                                .OrderByDescending(autoBid => autoBid.CreationDate)
                                .FirstOrDefault();
            if (latestAutoBid == null)
                return true;

            if (isFinalPriceSold)
            {
                return latestAutoBid.MaxPrice < createAutoBidDTO.MinPrice &&
                       createAutoBidDTO.MinPrice < createAutoBidDTO.MaxPrice &&
                       latestAutoBid.CustomerLot.Lot.FinalPriceSold <= createAutoBidDTO.MaxPrice;
            }
            else
            {
                return latestAutoBid.MaxPrice < createAutoBidDTO.MinPrice &&
                       latestAutoBid.MinPrice < createAutoBidDTO.MaxPrice;
            }
        }

        private APIResponseModel CreateErrorResponse(string message, int code)
        {
            return new APIResponseModel
            {
                Message = message,
                Code = code,
                IsSuccess = false
            };
        }

    }
}
