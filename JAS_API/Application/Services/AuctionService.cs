using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.AuctionDTOs;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Entity;
using Domain.Enums;

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
        private readonly ICacheService _cacheService;
        private readonly IWalletService _walletService;
        private readonly IServiceProvider _serviceProvider;

        public AuctionService(IUnitOfWork unitOfWork, IMapper mapper, IClaimsService claimsService, ICurrentTime currentTime, Cloudinary cloudinary, ILotService lotService, ICacheService cacheService, IWalletService walletService, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _claimsService = claimsService;
            _currentTime = currentTime;
            _cloudinary = cloudinary;
            _lotService = lotService;
            _cacheService = cacheService;
            _walletService = walletService;
            _serviceProvider = serviceProvider;
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
                if (newAuction == null)
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
                if (await _unitOfWork.SaveChangeAsync() > 0)
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
                if (auctionExisted.Status == EnumStatusAuction.Waiting.ToString())
                {
                    _mapper.Map(updateAuctionDTO, auctionExisted);
                    auctionExisted.ModificationDate = DateTime.Now;
                    auctionExisted.ModificationBy = _claimsService.GetCurrentUserId;
                    auctionExisted.StartTime = updateAuctionDTO.StartTime;
                    foreach (var lot in auctionExisted?.Lots)
                    {
                        lot.StartTime = updateAuctionDTO.StartTime;
                        lot.EndTime = updateAuctionDTO.EndTime;
                        _cacheService.UpdateLotEndTime(lot.Id, updateAuctionDTO.EndTime.Value);
                        _cacheService.UpdateLotStartTime(lot.Id, updateAuctionDTO.StartTime.Value);
                    }
                }
                else
                {
                    reponse.IsSuccess = false;
                    reponse.Message = $"Auction cannot update informaiton. Because aution are {auctionExisted.Status.ToString()} ";
                    reponse.Code = 400;
                    return reponse;
                }

                if (updateAuctionDTO.FileImage != null)
                {
                    var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams
                    {
                        File = new FileDescription(updateAuctionDTO.FileImage.FileName,
                              updateAuctionDTO.FileImage.OpenReadStream()),
                        Tags = Tags
                    }).ConfigureAwait(false);
                    if (uploadResult == null || uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        reponse.Message = $"File upload failed." + uploadResult?.Error.Message + "";
                        reponse.Code = (int)uploadResult.StatusCode;
                        reponse.IsSuccess = false;
                    }
                    auctionExisted.ImageLink = uploadResult.SecureUrl.AbsoluteUri;
                }

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
                    await _lotService.UpdateLotRange(auctionExisted.Id, EnumStatusAuction.UpComing.ToString());
                    _unitOfWork.AuctionRepository.Update(auctionExisted);
                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        reponse.IsSuccess = true;
                        reponse.Message = "Auction updated successfully.";
                        reponse.Code = 201;
                        reponse.Data = _mapper.Map<AuctionDTO>(auctionExisted);
                        return reponse;
                    }
                    else
                    {
                        reponse.IsSuccess = false;
                        reponse.Message = "Auction updated Faild.";
                        reponse.Code = 400;
                        return reponse;
                    }
                }
                else
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "The current time has exceeded the start time";
                    reponse.Code = 400;
                    return reponse;
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

        public async Task<APIResponseModel> CancelAuctionAndRangeLot(int auctionId)
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
                else
                {
                    switch (auctionExisted.Status)
                    {
                        case nameof(EnumStatusAuction.UpComing):
                            auctionExisted.Status = EnumStatusAuction.Canceled.ToString();
                            auctionExisted.ModificationDate = DateTime.Now;
                            await SetAfterCancelForLot(auctionExisted);
                            break;
                        case nameof(EnumStatusAuction.Live):
                            auctionExisted.Status = EnumStatusAuction.Canceled.ToString();
                            auctionExisted.ModificationDate = DateTime.Now;
                            await SetAfterCancelForLot(auctionExisted);
                            break;
                        default:
                            break;
                    }
                    _unitOfWork.AuctionRepository.Update(auctionExisted);
                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        reponse.IsSuccess = true;
                        reponse.Message = "Auction updated successfully.";
                        reponse.Code = 201;
                        reponse.Data = _mapper.Map<AuctionDTO>(auctionExisted);
                        return reponse;
                    }
                    else
                    {
                        reponse.IsSuccess = false;
                        reponse.Message = "Aution cannt saving because some condition.";
                    }
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
        private async Task SetAfterCancelForLot(Auction auction)
        {
            var lots = auction?.Lots?.ToList();
            if (lots.Any())
            {
                // Set Lot
                var tasks = lots.Select(async lot =>
                {
                    lot.Status = EnumStatusLot.Canceled.ToString();
                    List<CustomerLot> players = lot.CustomerLots.ToList();
                    if (players.Count > 0)
                    {
                        var tasksPlayers = players.Select(async player =>
                        {
                            player.Status = EnumCustomerLot.Refunded.ToString();
                            player.IsWinner = false;
                            player.IsRefunded = true;
                            //var notification = new Notification
                            //{
                            //    Title = $"Bidding lose in lot {player.LotId}",
                            //    Description = $" You had been lose in lot {player.LotId} và system auto refunded deposit for you",
                            //    Is_Read = false,
                            //    NotifiableId = player.Id,
                            //    AccountId = player.Customer.AccountId,
                            //    CreationDate = DateTime.UtcNow,
                            //    Notifi_Type = "Refunded",
                            //    ImageLink = player.Lot.Jewelry.ImageJewelries.FirstOrDefault()?.ImageLink
                            //};

                            //await _unitOfWork.NotificationRepository.AddAsync(notification);
                            //await _notificationHub.Clients.Group(player.Customer.AccountId.ToString()).SendAsync("NewNotificationReceived", "Có thông báo mới!");
                        });
                        await Task.WhenAll(tasksPlayers);
                        await _walletService.RefundToWalletForUsersAsync(players);
                    }
                    return;
                });
                await Task.WhenAll(tasks);
            }
        }
    }
}
