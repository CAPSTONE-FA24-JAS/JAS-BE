

using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.LotDTOs;
using AutoMapper;
using Domain.Entity;
using Domain.Enums;

namespace Application.Services
{
    public class LotService : ILotService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LotService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<APIResponseModel> CreateLot(object lotDTO)
        {
            var reponse = new APIResponseModel();
            try
            {
                if (lotDTO == null)
                {
                    reponse.Code = 404;
                    reponse.IsSuccess = false;
                    reponse.Message = "Not Found Lot";
                }
                else
                {
                    var lot = _mapper.Map<Lot>(lotDTO);
                    if (lotDTO is CreateLotFixedPriceDTO)
                    {
                        lot.LotType = EnumLotType.Fixed_Price.ToString();
                    }
                    if (lotDTO is CreateLotSecretAuctionDTO)
                    {
                        lot.LotType = EnumLotType.Secret_Auction.ToString();
                    }
                    if (lotDTO is CreateLotPublicAuctionDTO)
                    {
                        lot.LotType = EnumLotType.Public_Auction.ToString();
                    }
                    if (lotDTO is CreateLotAuctionPriceGraduallyReducedDTO)
                    {
                        lot.LotType = EnumLotType.Auction_Price_GraduallyReduced.ToString();
                    }
                    lot.Status = EnumStatusLot.Pending.ToString();
                    lot.FloorFeePercent = 25;

                    await _unitOfWork.LotRepository.AddAsync(lot);
                    if(await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        reponse.Code = 200;
                        reponse.IsSuccess = true;
                        reponse.Message = $"CreateLot {lot.LotType} is successfuly";
                    }
                    else
                    {
                        reponse.Code = 409;
                        reponse.IsSuccess = false;
                        reponse.Message = "Error when saving change";
                    }
                    
                }
            }
            catch (Exception e)
            {
                reponse.Code = 500;
                reponse.IsSuccess = false;
                reponse.ErrorMessages = new List<string> { e.Message };
            }
            return reponse;
        }

        public async Task<APIResponseModel> GetLotTypeById(int lotTypeId)
        {
            var reponse = new APIResponseModel();
            try
            {
                var lotTypeName = EnumHelper.GetEnums<EnumLotType>().FirstOrDefault(x => x.Value == lotTypeId).Name;
                if (lotTypeName == null)
                {
                    reponse.Code = 404;
                    reponse.IsSuccess = false;
                    reponse.Message = "Not Found LotType";
                }
                else
                {
                    reponse.Code = 200;
                    reponse.Data = lotTypeName;
                    reponse.IsSuccess = true;
                    reponse.Message = "Received list lot type is successfuly";
                }
            }
            catch (Exception e)
            {
                reponse.Code = 500;
                reponse.IsSuccess = false;
                reponse.ErrorMessages = new List<string> { e.Message };
            }
            return reponse;
        }

        public async Task<APIResponseModel> GetLotTypes()
        {
            var reponse = new APIResponseModel();
            try
            {
                var lotTypes = EnumHelper.GetEnums<EnumLotType>();
                if (!lotTypes.Any())
                {
                    reponse.Code = 404;
                    reponse.IsSuccess = false;
                    reponse.Message = "Not Found LotType";
                }
                else
                {
                    reponse.Code = 200;
                    reponse.Data = lotTypes;
                    reponse.IsSuccess = true;
                    reponse.Message = $"Received list lot type is successfuly. Have {lotTypes.Count} Type";
                }
            }
            catch (Exception e)
            {
                reponse.Code = 500;
                reponse.IsSuccess = false;
                reponse.ErrorMessages = new List<string> { e.Message };
            }
            return reponse;
        }
    }
}
