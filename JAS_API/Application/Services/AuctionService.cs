using Application.Interfaces;
using Application.ServiceReponse;
using Application.ViewModels.AuctionDTOs;
using AutoMapper;
using Azure;
using Domain.Entity;
using Domain.Enums;
using Google.Apis.Util;

namespace Application.Services
{
    public class AuctionService : IAuctionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClaimsService _claimsService;
        private readonly ICurrentTime _currentTime;

        public AuctionService(IUnitOfWork unitOfWork, IMapper mapper, IClaimsService claimsService, ICurrentTime currentTime)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _claimsService = claimsService;
            _currentTime = currentTime;
        }

        public async Task<APIResponseModel> CreateAuction(CreateAuctionDTO createAuctionDTO)
        {
            var reponse = new APIResponseModel();
            try
            {
                if (createAuctionDTO == null)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Auction is null";
                    reponse.Code = 404;
                    return reponse;
                }
                var newAuction = _mapper.Map<Auction>(createAuctionDTO);
                if(newAuction == null)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Auction is wrong when mapping";
                    reponse.Code = 500;
                    return reponse;
                }
                await _unitOfWork.AuctionRepository.AddAsync(newAuction);
                if(await _unitOfWork.SaveChangeAsync() > 0)
                {
                    reponse.IsSuccess = true;
                    reponse.Message = "Auction created successfully.";
                    reponse.Code = 201;
                    reponse.Data = createAuctionDTO;
                    return reponse;
                }
                reponse.IsSuccess = false;
                reponse.Message = "Auction create faild when saving.";
                reponse.Code = 500;
            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.Message = "Exception Eror";
                reponse.ErrorMessages = new List<string> { e.Message };
                reponse.Code = 500;
            }
            return reponse;
        }

        public async Task<APIResponseModel> DeleteSolfAuction(int Id)
        {
            var reponse = new APIResponseModel();
            try
            {
                var auctionExisted = await _unitOfWork.AuctionRepository.GetByIdAsync(Id);
                if (auctionExisted == null)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Auction Not Found";
                    reponse.Code = 404;
                    return reponse;
                }
                if (auctionExisted.IsDeleted == true)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Auction Is Deleted";
                    reponse.Code = 204;
                    return reponse;
                }
                auctionExisted.Status = EnumStatusAuction.Past.ToString();
                auctionExisted.DeletionDate = DateTime.Now;
                auctionExisted.IsDeleted = true;
                auctionExisted.CreatedBy = _claimsService.GetCurrentUserId;

                _unitOfWork.AuctionRepository.Update(auctionExisted);
                if (await _unitOfWork.SaveChangeAsync() > 0)
                {
                    reponse.IsSuccess = true;
                    reponse.Message = "Auction soft deleted successfully.";
                    reponse.Code = 201;
                    reponse.Data = _mapper.Map<AuctionDTO>(auctionExisted);
                    return reponse;
                }
                reponse.IsSuccess = false;
                reponse.Message = "Auction soft deleted faild when saving.";
                reponse.Code = 500;
            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.Message = "Exception Eror";
                reponse.ErrorMessages = new List<string> { e.Message };
                reponse.Code = 500;
            }
            return reponse;
        }

        public async Task<APIResponseModel> GetAuctionById(int Id)
        {
            var reponse = new APIResponseModel();
            try
            {
                var auctionExisted = await _unitOfWork.AuctionRepository.GetByIdAsync(Id);
                if (auctionExisted == null)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Auction Not Found";
                    reponse.Code = 404;
                    return reponse;
                }
                var autionDTO = _mapper.Map<AuctionDTO>(auctionExisted);
                if (autionDTO == null)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Received Auction Wrong When Mapping ";
                    reponse.Code = 500;
                    return reponse;
                }
                reponse.IsSuccess = true;
                reponse.Message = "Auction received successfully.";
                reponse.Code = 201;
                reponse.Data = autionDTO;
            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.Message = "Exception Eror";
                reponse.ErrorMessages = new List<string> { e.Message };
                reponse.Code = 500;
            }
            return reponse;
        }

        public async Task<APIResponseModel> UpdateAuction(UpdateAuctionDTO updateAuctionDTO)
        {
            var reponse = new APIResponseModel();
            try
            {
                var auctionExisted = await _unitOfWork.AuctionRepository.GetByIdAsync(updateAuctionDTO.AutionId);
                if (auctionExisted == null)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Auction Not Found";
                    reponse.Code = 404;
                    return reponse;
                }
                if (auctionExisted.StartTime <= _currentTime.GetCurrentTime() && auctionExisted.EndTime >= _currentTime.GetCurrentTime())
                {
                    var starttime = auctionExisted.StartTime;
                    _mapper.Map(updateAuctionDTO, auctionExisted);
                    auctionExisted.Status = EnumStatusAuction.Living.ToString();
                    auctionExisted.ModificationDate = DateTime.Now;
                    auctionExisted.ModificationBy = _claimsService.GetCurrentUserId;
                    auctionExisted.StartTime = starttime;
                }
                else if (auctionExisted.StartTime > _currentTime.GetCurrentTime())
                {
                    auctionExisted.Status = EnumStatusAuction.NotStarted.ToString();
                    auctionExisted.ModificationDate = DateTime.Now;
                    auctionExisted.ModificationBy = _claimsService.GetCurrentUserId;
                    _mapper.Map(updateAuctionDTO, auctionExisted);
                }

                //else if (auctionExisted.EndTime < _currentTime.GetCurrentTime())
                //{
                //    //_unitOfWork.AuctionRepository.SetPropertyModified(auctionExisted, "StartTime");
                //    //_unitOfWork.AuctionRepository.SetPropertyModified(auctionExisted, "EndTime");
                //    //_unitOfWork.AuctionRepository.SetPropertyModified(auctionExisted, "Description");
                //    //_unitOfWork.AuctionRepository.SetPropertyModified(auctionExisted, "Location");
                //    //_unitOfWork.AuctionRepository.SetPropertyModified(auctionExisted, "Notes");
                //    auctionExisted.Status = EnumStatusAuction.Past.ToString();
                //    auctionExisted.ModificationDate = DateTime.Now;
                //    auctionExisted.ModificationBy = _claimsService.GetCurrentUserId;
                //}
                
                _unitOfWork.AuctionRepository.Update(auctionExisted);
                if (await _unitOfWork.SaveChangeAsync() > 0)
                {
                    reponse.IsSuccess = true;
                    reponse.Message = "Auction updated successfully.";
                    reponse.Code = 201;
                    reponse.Data = _mapper.Map<AuctionDTO>(auctionExisted);
                    return reponse;
                }
                reponse.IsSuccess = false;
                reponse.Message = "Auction updated faild when saving.";
                reponse.Code = 500;
            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.Message = "Exception Eror";
                reponse.ErrorMessages = new List<string> { e.Message };
                reponse.Code = 500;
            }
            return reponse;
        }
        public async Task<APIResponseModel> ViewAutions()
        {
            var reponse = new APIResponseModel();
            try
            {
                var auctionsExisted = await _unitOfWork.AuctionRepository.GetAllAsync();
                if (!auctionsExisted.Any())
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Auctions Not Found";
                    reponse.Code = 404;
                    return reponse;
                }
                var autionDTO = _mapper.Map<IEnumerable<AuctionDTO>>(auctionsExisted);
                if (autionDTO == null)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Received Auctions Wrong When Mapping ";
                    reponse.Code = 500;
                    return reponse;
                }
                reponse.IsSuccess = true;
                reponse.Message = "Auctions received successfully.";
                reponse.Code = 201;
                reponse.Data = autionDTO;
            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.Message = "Exception Eror";
                reponse.ErrorMessages = new List<string> { e.Message };
                reponse.Code = 500;
            }
            return reponse;
        }
    }
}
