using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.AuctionDTOs;
using Application.ViewModels.BidLimitDTOs;
using AutoMapper;
using Azure;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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
        private readonly Cloudinary _cloudinary;
        private const string Tags = "Backend_ImageAuction";
        private readonly ILotService _lotService;

        public AuctionService(IUnitOfWork unitOfWork, IMapper mapper, IClaimsService claimsService, ICurrentTime currentTime, Cloudinary cloudinary, ILotService lotService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _claimsService = claimsService;
            _currentTime = currentTime;
            _cloudinary = cloudinary;
            _lotService = lotService;
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
                newAuction.Status = EnumStatusAuction.Waiting.ToString();
                if(newAuction == null)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Auction is wrong when mapping";
                    reponse.Code = 500;
                    return reponse;
                }
                var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams
                {
                    File = new FileDescription(createAuctionDTO.FileImage.FileName,
                               createAuctionDTO.FileImage.OpenReadStream()),
                    Tags = Tags
                }).ConfigureAwait(false);
                if (uploadResult == null || uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    reponse.Message = $"File upload failed." + uploadResult.Error.Message + "";
                    reponse.Code = (int)uploadResult.StatusCode;
                    reponse.IsSuccess = false;
                }
                newAuction.ImageLink = uploadResult.SecureUrl.AbsoluteUri;
                await _unitOfWork.AuctionRepository.AddAsync(newAuction);
                if(await _unitOfWork.SaveChangeAsync() > 0)
                {
                    reponse.IsSuccess = true;
                    reponse.Message = "Auction created successfully.";
                    reponse.Code = 201;
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

        public async Task<APIResponseModel> GetAuctionByStatus(int valueId)
        {
            var reponse = new APIResponseModel();
            try
            {
                var status = EnumHelper.GetEnums<EnumStatusAuction>().FirstOrDefault(x => x.Value == valueId).Name;
                if (status != null)
                {
                    var auctionExisteds = await _unitOfWork.AuctionRepository.GetAllAsync(condition: x => x.Status == status);
                    if (auctionExisteds == null)
                    {
                        reponse.IsSuccess = false;
                        reponse.Message = "Auctions Not Found";
                        reponse.Code = 404;
                        return reponse;
                    }
                    var autionDTOs = _mapper.Map<IEnumerable<AuctionDTO>>(auctionExisteds);
                    if (autionDTOs == null)
                    {
                        reponse.IsSuccess = false;
                        reponse.Message = "Received Auctions Wrong When Mapping ";
                        reponse.Code = 500;
                        return reponse;
                    }
                    reponse.IsSuccess = true;
                    reponse.Message = "Auctions received successfully.";
                    reponse.Code = 201;
                    reponse.Data = autionDTOs;
                }
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

        public async Task<APIResponseModel> GetStatusAuction()
        {
            var reponse = new APIResponseModel();
            try
            {
                var filters = EnumHelper.GetEnums<EnumStatusAuction>();
                if (filters == null)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Receive Filter Bid Limit Fail";
                    reponse.Code = 404;
                }
                else
                {
                    reponse.IsSuccess = true;
                    reponse.Message = "Receive Filter Bid Limit Successfull";
                    reponse.Code = 200;
                    reponse.Data = filters;
                }
            }
            catch (Exception e)
            {
                reponse.IsSuccess = true;
                reponse.Message = e.Message;
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
                    auctionExisted.Status = EnumStatusAuction.Live.ToString();
                    auctionExisted.ModificationDate = DateTime.Now;
                    auctionExisted.ModificationBy = _claimsService.GetCurrentUserId;
                    auctionExisted.StartTime = starttime;
                }
                else if (auctionExisted.StartTime > _currentTime.GetCurrentTime())
                {
                    auctionExisted.Status = EnumStatusAuction.UpComing.ToString();
                    auctionExisted.ModificationDate = DateTime.Now;
                    auctionExisted.ModificationBy = _claimsService.GetCurrentUserId;
                    await _lotService.UpdateLotRange(auctionExisted.Id);
                    _mapper.Map(updateAuctionDTO, auctionExisted);
                }
                var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams
                {
                    File = new FileDescription(updateAuctionDTO.FileImage.FileName,
                              updateAuctionDTO.FileImage.OpenReadStream()),
                    Tags = Tags
                }).ConfigureAwait(false);
                if (uploadResult == null || uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    reponse.Message = $"File upload failed." + uploadResult.Error.Message + "";
                    reponse.Code = (int)uploadResult.StatusCode;
                    reponse.IsSuccess = false;
                }
                auctionExisted.ImageLink = uploadResult.SecureUrl.AbsoluteUri;
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

        public async Task<APIResponseModel> ApproveAuction(int auctionId)
        {
            var reponse = new APIResponseModel();
            try
            {
                var auctionExisted = await _unitOfWork.AuctionRepository.GetByIdAsync(auctionId);
                if (auctionExisted == null)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Auction Not Found";
                    reponse.Code = 404;
                    return reponse;
                }
                if (auctionExisted.StartTime > _currentTime.GetCurrentTime())
                {
                    auctionExisted.Status = EnumStatusAuction.UpComing.ToString();
                    auctionExisted.ModificationDate = DateTime.Now;
                    auctionExisted.ModificationBy = _claimsService.GetCurrentUserId;
                    await _lotService.UpdateLotRange(auctionExisted.Id);
                    _unitOfWork.AuctionRepository.Update(auctionExisted);
                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        reponse.IsSuccess = true;
                        reponse.Message = "Auction updated successfully.";
                        reponse.Code = 201;
                        reponse.Data = _mapper.Map<AuctionDTO>(auctionExisted);
                        return reponse;
                    }
                }
                reponse.IsSuccess = false;
                reponse.Message = "Aution cannt saving because some condition.";
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
