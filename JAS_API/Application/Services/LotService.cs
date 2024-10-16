

using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.LotDTOs;
using AutoMapper;
using Domain.Entity;
using Domain.Enums;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System.Linq.Expressions;

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
                    var check = (ValueTuple<bool, string>)CheckExitProperty(lot);
                    if (check.Item1 == false)
                    {
                        reponse.Code = 402;
                        reponse.IsSuccess = false;
                        reponse.Message = "Jewlry was into another lot";
                    }
                    else
                    {
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

                        lot.Status = EnumStatusLot.Creaeted.ToString();
                        lot.FloorFeePercent = 25;
                        await _unitOfWork.LotRepository.AddAsync(lot);
                        var jewelry = await _unitOfWork.JewelryRepository.GetByIdAsync(lot.JewelryId);
                        jewelry.Status = EnumStatusJewelry.Added.ToString();
                        if (await _unitOfWork.SaveChangeAsync() > 0)
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
            }
            catch (Exception e)
            {
                reponse.Code = 500;
                reponse.IsSuccess = false;
                reponse.ErrorMessages = new List<string> { e.Message };
            }
            return reponse;
        }

        public async Task<APIResponseModel> GetLotById(int Id)
        {
            var reponse = new APIResponseModel();
            try
            {
                var lot = await _unitOfWork.LotRepository.GetByIdAsync( Id, includes: new Expression<Func<Lot, object>>[] { x => x.Staff, x => x.Seller, x => x.Jewelry });
                if (lot == null)
                {
                    reponse.Code = 404;
                    reponse.IsSuccess = false;
                    reponse.Message = "Not Found Lots";
                }
                else
                {
                    reponse.Code = 200;
                    reponse.Data = IsMapper(lot, lot.LotType);
                    reponse.IsSuccess = true;
                    reponse.Message = $"Received lot is successfuly.";
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

        public async Task<APIResponseModel> GetLots()
        {
            var reponse = new APIResponseModel();
            try
            {
                var lots = await _unitOfWork.LotRepository.GetAllAsync(includes : new Expression<Func<Lot, object>>[] {x => x.Staff, x => x.Seller, x => x.Jewelry });
                if (!lots.Any())
                {
                    reponse.Code = 404;
                    reponse.IsSuccess = false;
                    reponse.Message = "Not Found Lots";
                }
                else
                {
                    reponse.Code = 200;
                    var DTOs =  new List<object>();
                    foreach (var lot in lots)
                    {
                        var mapper = IsMapper(lot, lot.LotType);
                        DTOs.Add(mapper);
                    }
                    reponse.Data = DTOs;
                    reponse.IsSuccess = true;
                    reponse.Message = $"Received list lot is successfuly. Have {lots.Count} lot";
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

        internal object IsMapper(Lot lot, string lotType)
        {
            object result;

            switch (lotType)
            {
                case nameof(EnumLotType.Fixed_Price):
                    result = _mapper.Map<LotFixedPriceDTO>(lot);
                    break;
                case nameof(EnumLotType.Secret_Auction):
                    result = _mapper.Map<LotSecretAuctionDTO>(lot);
                    break;
                case nameof(EnumLotType.Public_Auction):
                    result = _mapper.Map<LotPublicAuctionDTO>(lot);
                    break;
                case nameof(EnumLotType.Auction_Price_GraduallyReduced):
                    result = _mapper.Map<LotAuctionPriceGraduallyReducedDTO>(lot);
                    break;
                default:
                    throw new ArgumentException($"Unsupported lot type: {lotType}");
            }

            return result;
        }
        internal object CheckExitProperty(Lot lot)
        {
            object result = (status: true, msg: "Status is suiable");
            if (_unitOfWork.JewelryRepository.GetByIdAsync(lot.JewelryId).Result.Status != null)
            {
                result = (status: false, msg: "jewelry cannot add to lot because it was sold or add to another lot");
            }
            return result;

        }

        public async Task<APIResponseModel> GetLotByAuctionId(int auctionId)
        {
            var reponse = new APIResponseModel();
            try
            {
                var lots = await _unitOfWork.LotRepository.GetAllAsync( condition: x => x.AuctionId == auctionId, includes: new Expression<Func<Lot, object>>[] { x => x.Staff, x => x.Seller, x => x.Jewelry });
                if (!lots.Any())
                {
                    reponse.Code = 404;
                    reponse.IsSuccess = false;
                    reponse.Message = "Not Found Lots";
                }
                else
                {
                    reponse.Code = 200;
                    reponse.Data = _mapper.Map<IEnumerable<LotDTO>>(lots);
                    reponse.IsSuccess = true;
                    reponse.Message = $"Received lots is successfuly. Have {lots.Count} lot in Auction";
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
