using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.AccountDTOs;
using Application.ViewModels.LotDTOs;
using AutoMapper;
using Azure;
using Domain.Entity;
using Domain.Enums;
using Google.Apis.Storage.v1.Data;
using System.Linq.Expressions;
using static Google.Apis.Requests.BatchRequest;

namespace Application.Services
{
    public class LotService : ILotService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IAccountService _accountService;
        private readonly IWalletService _walletService;
        private readonly IWalletTransactionService _walletTransactionService;

        public LotService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService, IAccountService accountService, IWalletService walletService, IWalletTransactionService walletTransactionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
            _accountService = accountService;
            _walletService = walletService;
            _walletTransactionService = walletTransactionService;
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
                    //var check =  (ValueTuple<bool, string>)await CheckExitProperty(lot);
                    //if (check.Item1 == false)
                    //{
                    //    reponse.Code = 402;
                    //    reponse.IsSuccess = false;
                    //    reponse.Message = "Jewlry was into another lot";
                    //}
                    var autionexits =await _unitOfWork.AuctionRepository.GetByIdAsync(lot.AuctionId);
                    if(autionexits != null)
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

                            lot.StartTime = autionexits.StartTime;
                            lot.EndTime = autionexits.EndTime;
                            lot.Status = EnumStatusLot.Waiting.ToString();
                            lot.FloorFeePercent = 25;
                            await _unitOfWork.LotRepository.AddAsync(lot);
                            var jewelry = await _unitOfWork.JewelryRepository.GetByIdAsync(lot.JewelryId);
                            jewelry.Status = EnumStatusJewelry.Added.ToString();
                            if (await _unitOfWork.SaveChangeAsync() > 0)
                            {
                            // Lưu lot vào Redis(dung hash)
                            var lotRedis = new Lot
                            {
                                StartTime = lot.StartTime,
                                EndTime = lot.EndTime,
                                Id = lot.Id,
                                Status = lot.Status,
                                AuctionId = lot.AuctionId
                            };
                            _cacheService.SetLotInfo(lotRedis);

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
                    else
                    {
                        reponse.Code = 404;
                        reponse.IsSuccess = false;
                        reponse.Message = "Auction not found";
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
        internal async Task<object> CheckExitProperty(Lot lot)
        {
            object result = (status: true, msg: "Status is suiable");
            var jewelry = await _unitOfWork.JewelryRepository.GetByIdAsync(lot.JewelryId);
            if (jewelry.Status != null)
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

        public async Task<APIResponseModel> GetListStatusOfLot()
        {
            var reponse = new APIResponseModel();
            try
            {
                var filters = EnumHelper.GetEnums<EnumStatusLot>();
                if (filters == null)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Receive Status Of Lot Fail";
                    reponse.Code = 400;
                }
                else
                {
                    reponse.IsSuccess = true;
                    reponse.Message = "Receive Status Of Lot Successfull";
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

        public async Task<APIResponseModel> RegisterToLot(RegisterToLotDTO registerToLotDTO)
        {
            var response = new APIResponseModel();
            try
            {
                var lotExist = await _unitOfWork.LotRepository.GetByIdAsync(registerToLotDTO.LotId);
                var checkBidLimit = new APIResponseModel();
                if (lotExist == null)
                {
                    return new APIResponseModel { IsSuccess = false, Message = "Not Found Lot.", Code = 404 };
                }

                if (lotExist.HaveFinancialProof == true)
                {
                    checkBidLimit = await _accountService.CheckBidLimit((int)registerToLotDTO.CustomerId);
                    if (!checkBidLimit.IsSuccess)
                    {
                        return checkBidLimit;
                    }
                }

                var depositOfLot = lotExist.Deposit;
                var checkWallet = await _walletService.CheckWalletExist((int)registerToLotDTO.CustomerId, (float)depositOfLot);
                if (!checkWallet.IsSuccess)
                {
                    return checkWallet;
                }

                if (checkWallet.Data is Wallet wallet)
                {
                    var minusDeposit = await _walletService.UpdateBanlance(wallet.Id, (decimal)depositOfLot, false);
                    if (!minusDeposit.IsSuccess)
                    {
                        return minusDeposit;
                    }

                    var customerLot = _mapper.Map<CustomerLot>(registerToLotDTO);
                    customerLot.IsDeposit = true;

                    var newTransactionWallet = new WalletTransaction
                    {
                        Amount = -depositOfLot,
                        DocNo = customerLot.Id,
                        Status = "Completed",
                        transactionType = EnumTransactionType.DepositWallet.ToString(),
                        TransactionTime = DateTime.UtcNow,
                    };

                    var newTransactionCompany = new Transaction
                    {
                        Amount = depositOfLot,
                        DocNo = customerLot.Id,
                        TransactionType = EnumTransactionType.DepositWallet.ToString(),
                        TransactionTime = DateTime.UtcNow,
                    };

                    await _unitOfWork.WalletTransactionRepository.AddAsync(newTransactionWallet);
                    await _unitOfWork.TransactionRepository.AddAsync(newTransactionCompany);

                    if (checkBidLimit.Data is Customer customer)
                    {
                        customerLot.PriceLimit = customer.PriceLimit;
                        customerLot.ExpireDateOfBidLimit = customer.ExpireDate;
                    }

                    customerLot.Status = EnumHelper.GetEnums<EnumCustomerLot>().FirstOrDefault(x => x.Value == 1).Name;
                    await _unitOfWork.CustomerLotRepository.AddAsync(customerLot);

                    await AddBidPriceIfApplicable(lotExist.LotType, registerToLotDTO);

                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        response.IsSuccess = true;
                        response.Message = "Register customer to lot successfully";
                        response.Code = 200;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "Failed to register customer to lot";
                        response.Code = 400;
                    }
                }
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + e.Message;
                response.Code = 500;
            }
            return response;
        }

        private async Task AddBidPriceIfApplicable(string lotType, RegisterToLotDTO registerToLotDTO)
        {
            var bidPrice = new BidPrice
            {
                CustomerId = (int)registerToLotDTO.CustomerId,
                LotId = (int)registerToLotDTO.LotId,
                CurrentPrice = registerToLotDTO.CurrentPrice,
                BidTime = DateTime.UtcNow
            };

            if (lotType == EnumLotType.Fixed_Price.ToString() || lotType == EnumLotType.Secret_Auction.ToString())
            {
                await _unitOfWork.BidPriceRepository.AddAsync(bidPrice);
            }
        }
        public async Task<APIResponseModel> GetCustomerLotByLot(int lotId)
        {
            var reponse = new APIResponseModel();
            try
            {
                var lots = await _unitOfWork.CustomerLotRepository.GetAllAsync(condition: x => x.LotId == lotId);
                if(lots.Count() > 0)
                {
                    reponse.Code = 200;
                    reponse.Data = _mapper.Map<IEnumerable<CustomerLotDTO>>(lots);
                    reponse.IsSuccess = true;
                    reponse.Message = $"Received Customer lot is successfuly.";
                }
                else
                {
                    reponse.Code = 400;
                    reponse.IsSuccess = true;
                    reponse.Message = $"Received Customer lot is empty.";

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
        public async Task<APIResponseModel> CheckCustomerInLot(int customerId, int lotId)
        {
            var reponse = new APIResponseModel();
            try
            {
                var lots = await _unitOfWork.CustomerLotRepository.GetAllAsync(condition: x => x.LotId == lotId && x.CustomerId == customerId);
                if (lots.Count() > 0)
                {
                    reponse.Code = 200;
                    reponse.Data = true;
                    reponse.IsSuccess = true;
                    reponse.Message = $"Customer was joined to lot";
                }
                else
                {
                    reponse.Code = 400;
                    reponse.IsSuccess = true;
                    reponse.Message = $"Customer havent in lot.";

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
        public async Task<APIResponseModel> UpdateLotRange(int auctionId)
        {
            var response = new APIResponseModel();
            try
            {
                var lots = await _unitOfWork.LotRepository.GetAllAsync(condition: x => x.AuctionId == auctionId);
                if (lots != null && lots.Any())
                {
                    foreach (var lot in lots)
                    {
                        lot.Status = EnumStatusLot.Ready.ToString();
                    }

                    //cho lên redis update staus lot 1 loạt
                    _unitOfWork.LotRepository.UpdateRange(lots);
                    await _unitOfWork.SaveChangeAsync(); 
                    response.Code = 200;
                    response.Data = true;
                    response.IsSuccess = true;
                    response.Message = $"Successfully updated lots for auction {auctionId}.";
                }
                else
                {
                    response.Code = 404;
                    response.IsSuccess = false;
                    response.Message = $"No lots found for auction {auctionId}.";
                }
            }
            catch (Exception e)
            {
                response.Code = 500;
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { e.Message };
            }
            return response;
        }
    }
}
