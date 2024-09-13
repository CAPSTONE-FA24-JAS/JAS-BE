using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.BidLimitDTOs;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Entity;
using Domain.Enums;
using Microsoft.Identity.Client;

namespace Application.Services
{
    public class BidLimitService : IBidLimitService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private const string Tags = "Backend_FileBidLimit";

        public BidLimitService(IUnitOfWork unitOfWork, IMapper mapper, Cloudinary cloudinary)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinary = cloudinary;
        }

        public async Task<APIResponseModel> CreateNewBidLimit(CreateBidLimitDTO createBidLimitDTO)
        {
            var reponse = new APIResponseModel();
            try
            {
                var uploadResult = await _cloudinary.UploadAsync(new RawUploadParams
                {
                    File = new FileDescription(createBidLimitDTO.File.FileName,
                                                createBidLimitDTO.File.OpenReadStream()),
                    Tags = Tags
                    ,
                    Type = "upload"
                }).ConfigureAwait(false);

                if (uploadResult == null || uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    reponse.Message = $"File upload failed." + uploadResult.Error.Message + "";
                    reponse.Code = (int)uploadResult.StatusCode;
                    reponse.IsSuccess = false;
                }
                else
                {
                    var enity = _mapper.Map<BidLimit>(createBidLimitDTO);
                    enity.File = uploadResult.SecureUrl.AbsoluteUri;
                    enity.ExpireDate = DateTime.Now.AddMonths(3);
                    enity.Status = null;
                     await _unitOfWork.BidLimitRepository.AddAsync(enity);
                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        reponse.Message = $"File upload Successfull";
                        reponse.Code = 200;
                        reponse.IsSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                reponse.ErrorMessages = ex.Message.Split(',').ToList();
                reponse.Message = "Exception";
                reponse.Code = 500;
                reponse.IsSuccess = false;
            }
            return reponse;
        }

        public async Task<APIResponseModel> ViewListBidLimit()
        {
            var reponse = new APIResponseModel();
            try
            {
                var bidLimits = await _unitOfWork.BidLimitRepository.GetAllAsync(includes: x => x.Account);
                if (bidLimits.Any())
                {
                    var DTOs = new List<BidLimitDTO>();
                    foreach (var bidLimit in bidLimits)
                    {
                        var mapper = _mapper.Map<BidLimitDTO>(bidLimit);
                        mapper.AccountName = bidLimit.Account.FirstName + " " + bidLimit.Account.LastName;
                        DTOs.Add(mapper);
                    }
                    if (DTOs.Count > 0)
                    {
                        reponse.Code = 200;
                        reponse.IsSuccess = true;
                        reponse.Message = "Received List BidLimit Successfull";
                        reponse.Data = DTOs;
                    }
                }
                else
                {
                    reponse.Code = 400;
                    reponse.IsSuccess = false;
                    reponse.Message = "Received List BidLimit Faild";
                }
            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.Message = "Exception";
                reponse.ErrorMessages = new List<string> { e.Message };
            }
            return reponse;
        }

        public async Task<APIResponseModel> ViewBidLimitByAccount(int accountId)
        {
            var reponse = new APIResponseModel();
            try
            {
                var bidLimits = await _unitOfWork.BidLimitRepository.GetAllAsync(condition: x => x.AccountId == accountId, includes: x => x.Account);
                if(bidLimits.Any()) 
                {
                    var DTOs = new List<BidLimitDTO>();
                    foreach (var bidLimit in bidLimits)
                    {
                        var mapper = _mapper.Map<BidLimitDTO>(bidLimit);
                        mapper.AccountName = bidLimit.Account.FirstName + " " + bidLimit.Account.LastName;
                        DTOs.Add(mapper);
                    }
                    if(DTOs.Count > 0)
                    {
                        reponse.Code = 200;
                        reponse.IsSuccess = true;
                        reponse.Message = "Received List BidLimit Successfull";
                        reponse.Data = DTOs;
                    }
                }
                else
                {
                    reponse.Code = 400;
                    reponse.IsSuccess = false;
                    reponse.Message = "Received List BidLimit Faild";
                }
            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.Message = "Exception";
                reponse.ErrorMessages = new List<string> { e.Message };
            }
            return reponse;
        }

        public async Task<APIResponseModel> UpdateStatus(UpdateBidLimitDTO updateBidLimitDTO)
        {
            var reponse = new APIResponseModel();
            try
            {
                var bidLimit = await _unitOfWork.BidLimitRepository.GetByIdAsync(updateBidLimitDTO.Id);
                if (bidLimit == null)
                {
                    reponse.Code = 400;
                    reponse.IsSuccess = false;
                    reponse.Message = "Not Found BidLimit";
                }
                else
                {
                    var status = EnumHelper.GetEnums<EnumStatusBidLimit>().FirstOrDefault(x => x.Value == updateBidLimitDTO.Status).Name;
                    if(status != null)
                    {
                        bidLimit.Status = status;
                        bidLimit.PriceLimit = updateBidLimitDTO.PriceLimit;
                        _unitOfWork.BidLimitRepository.Update(bidLimit);
                        if(await _unitOfWork.SaveChangeAsync() > 0)
                        {
                            reponse.Code = 200;
                            reponse.IsSuccess = true;
                            reponse.Message = $"Update Status : {status} Successfull";
                        }
                        else
                        {
                            reponse.IsSuccess = false;
                            reponse.Message = $"Errer when saving.";
                        }
                    }
                    else
                    {
                        reponse.Code = 400;
                        reponse.IsSuccess = false;
                        reponse.Message = "Not Found Status BidLimit";
                    }
                    
                }
            }
            catch (Exception e)
            {
                reponse.Code = 400;
                reponse.IsSuccess = false;
                reponse.ErrorMessages = new List<string> { e.Message };
            }
            return reponse;
        }

        public async Task<APIResponseModel> GetStatusBidLimt()
        {
            var reponse = new APIResponseModel();
            try
            {
                var filters = EnumHelper.GetEnums<EnumStatusBidLimit>();
                if (filters == null)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Receive Filter Bid Limit Fail";
                    reponse.Code = 400;
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

        public async Task<APIResponseModel> ViewBidLimitById(int Id)
        {
            var reponse = new APIResponseModel();
            try
            {
                var bidLimit = await _unitOfWork.BidLimitRepository.GetByIdAsync(Id,includes: x => x.Account);
                if (bidLimit != null)
                {
                        var mapper = _mapper.Map<BidLimitDTO>(bidLimit);
                        mapper.AccountName = bidLimit.Account.FirstName + " " + bidLimit.Account.LastName;
                        reponse.Code = 200;
                        reponse.IsSuccess = true;
                        reponse.Message = "Received List BidLimit Successfull";
                        reponse.Data = mapper;
                }
                else
                {
                    reponse.Code = 400;
                    reponse.IsSuccess = false;
                    reponse.Message = "Received List BidLimit Faild";
                }
            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.Message = "Exception";
                reponse.ErrorMessages = new List<string> { e.Message };
            }
            return reponse;
        }
    }
}
