using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.AuctionDTOs;
using Application.ViewModels.BidPriceDTOs;
using Application.ViewModels.CustomerLotDTOs;
using Application.ViewModels.LotDTOs;
using Application.ViewModels.NotificationDTOs;
using AutoMapper;
using Domain.Entity;
using Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using System.Linq.Expressions;
using WebAPI.Middlewares;

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
        private readonly IFoorFeePercentService _foorFeePercentService;
        private readonly IHubContext<BiddingHub> _hubContext;
        private readonly IHubContext<NotificationHub> _notificationHub;

        public LotService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService, IAccountService accountService, IWalletService walletService,
            IWalletTransactionService walletTransactionService, IFoorFeePercentService foorFeePercentService, IHubContext<BiddingHub> hubContext, IHubContext<NotificationHub> notificationHub)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
            _accountService = accountService;
            _walletService = walletService;
            _walletTransactionService = walletTransactionService;
            _foorFeePercentService = foorFeePercentService;
            _hubContext = hubContext;
            _notificationHub = notificationHub;
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
                    var autionexits = await _unitOfWork.AuctionRepository.GetByIdAsync(lot.AuctionId);
                    if (autionexits != null)
                    {

                        if (lotDTO is CreateLotFixedPriceDTO)
                        {
                            lot.LotType = EnumLotType.Fixed_Price.ToString();
                        }
                        if (lotDTO is CreateLotSecretAuctionDTO)
                        {
                            lot.LotType = EnumLotType.Secret_Auction.ToString();
                        }
                        if (lotDTO is CreateLotPublicAuctionDTO publicAuctionDTO)
                        {
                            if (!publicAuctionDTO.IsHaveFinalPrice.Value)
                            {
                                lot.FinalPriceSold = null;
                            }
                            lot.LotType = EnumLotType.Public_Auction.ToString();
                        }
                        if (lotDTO is CreateLotAuctionPriceGraduallyReducedDTO)
                        {
                            lot.LotType = EnumLotType.Auction_Price_GraduallyReduced.ToString();
                        }

                        lot.StartTime = autionexits.StartTime;
                        lot.EndTime = autionexits.EndTime;
                        lot.Status = EnumStatusLot.Waiting.ToString();
                        await _unitOfWork.LotRepository.AddAsync(lot);
                        var jewelry = await _unitOfWork.JewelryRepository.GetByIdAsync(lot.JewelryId);

                        if (await _unitOfWork.SaveChangeAsync() > 0)
                        {
                            // Lưu lot vào Redis(dung hash)
                            var lotRedis = new Lot
                            {
                                StartTime = lot.StartTime,
                                EndTime = lot.EndTime,
                                Id = lot.Id,
                                Status = lot.Status,
                                AuctionId = lot.AuctionId,
                                StartPrice = lot.StartPrice,
                                FinalPriceSold = lot.FinalPriceSold,
                                BidIncrement = lot.BidIncrement,
                                LotType = lot.LotType,
                                BidIncrementTime = lot.BidIncrementTime,
                                Title = lot.Title,
                                Deposit = lot.Deposit,
                                IsExtend = lot.IsExtend,
                                HaveFinancialProof = lot.HaveFinancialProof,
                                StaffId = lot.StaffId,
                                JewelryId = lot.JewelryId,
                                Round = lot.Round
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
                var lot = await _unitOfWork.LotRepository.GetByIdAsync(Id, includes: new Expression<Func<Lot, object>>[] { x => x.Staff, x => x.Seller, x => x.Jewelry });
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
                var lots = await _unitOfWork.LotRepository.GetAllAsync(includes: new Expression<Func<Lot, object>>[] { x => x.Staff, x => x.Seller, x => x.Jewelry });
                if (!lots.Any())
                {
                    reponse.Code = 404;
                    reponse.IsSuccess = true;
                    reponse.Message = "Not Found Lots";
                }
                else
                {
                    reponse.Code = 200;
                    var DTOs = new List<object>();
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
                    reponse.IsSuccess = true;
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
                //var lots = await _unitOfWork.LotRepository.GetAllAsync(condition: x => x.AuctionId == auctionId, includes: new Expression<Func<Lot, object>>[] { x => x.Staff, x => x.Seller, x => x.Jewelry });
                var auction = await _unitOfWork.AuctionRepository.GetByIdAsync(auctionId);
                if (auction == null)
                {
                    reponse.Code = 404;
                    reponse.IsSuccess = true;
                    reponse.Message = "Not Found Lots";
                }
                else
                {
                    var auctionDTO = _mapper.Map<AuctionDTO>(auction);
                    auctionDTO.SetLotDTOs(auction.Lots, _mapper);
                    reponse.Code = 200;
                    reponse.Data = auctionDTO;
                    reponse.IsSuccess = true;
                    reponse.Message = $"Received lots is successfuly. Have {auction.Lots.Count()} lot in Auction";
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
        public async Task<APIResponseModel> GetLotInAuctionId(int auctionId)
        {
            var reponse = new APIResponseModel();
            try
            {
                var lots = await _unitOfWork.LotRepository.GetAllAsync(condition: x => x.AuctionId == auctionId, includes: new Expression<Func<Lot, object>>[] { x => x.Staff, x => x.Seller, x => x.Jewelry });
                if (!lots.Any())
                {
                    reponse.Code = 404;
                    reponse.IsSuccess = true;
                    reponse.Message = "Not Found Lots";
                }
                else
                {
                    reponse.Code = 200;
                    reponse.Data = _mapper.Map<IEnumerable<LotDTO>>(lots); ;
                    reponse.IsSuccess = true;
                    reponse.Message = $"Received lots is successfuly. Have {lots.Count()} lot in Auction";
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
                    reponse.IsSuccess = true;
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

                if (registerToLotDTO.CustomerId == lotExist.Jewelry.Valuation.SellerId)
                {
                    return new APIResponseModel { IsSuccess = false, Message = "Seller Canot Auction Jewelry Of Him/Her.", Code = 400 };
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
                    customerLot.CurrentPrice = 0;
                    await _unitOfWork.CustomerLotRepository.AddAsync(customerLot);
                    await _unitOfWork.SaveChangeAsync();
                    var newTransactionWallet = new WalletTransaction
                    {
                        Amount = -depositOfLot,
                        DocNo = customerLot.Id,
                        Status = "Completed",
                        transactionType = EnumTransactionType.DepositWallet.ToString(),
                        TransactionTime = DateTime.UtcNow,
                        transactionPerson = (int)registerToLotDTO.CustomerId
                    };

                    var newTransactionCompany = new Transaction
                    {
                        Amount = depositOfLot,
                        DocNo = customerLot.Id,
                        TransactionType = EnumTransactionType.DepositWallet.ToString(),
                        TransactionTime = DateTime.UtcNow,
                        TransactionPerson = (int)registerToLotDTO.CustomerId
                    };

                    await _unitOfWork.WalletTransactionRepository.AddAsync(newTransactionWallet);
                    await _unitOfWork.TransactionRepository.AddAsync(newTransactionCompany);

                    if (checkBidLimit.Data is Customer customer)
                    {
                        customerLot.PriceLimit = customer.PriceLimit;
                        customerLot.ExpireDateOfBidLimit = customer.ExpireDate;
                    }

                    customerLot.Status = EnumCustomerLot.Registed.ToString();

                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        //noti cho staff quan ly lot
                        var notification = new Notification
                        {
                            Title = $"Have customer register to bid for lot",
                            Description = $" Have customer register to bid for lot {customerLot.Lot.Title}",
                            Is_Read = false,
                            NotifiableId = customerLot.LotId,  //LotId
                            AccountId = customerLot.Lot.Staff.AccountId,
                            CreationDate = DateTime.UtcNow,
                            Notifi_Type = "Registed",
                            ImageLink = customerLot.Lot.Jewelry.ImageJewelries.FirstOrDefault().ImageLink
                        };
                        await _unitOfWork.NotificationRepository.AddAsync(notification);
                        await _unitOfWork.SaveChangeAsync();
                        await _notificationHub.Clients.Group(customerLot.Lot.Staff.AccountId.ToString()).SendAsync("NewNotificationReceived", "Có thông báo mới!");
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

        public async Task<APIResponseModel> GetCustomerLotByLot(int lotId)
        {
            var reponse = new APIResponseModel();
            try
            {
                var lots = await _unitOfWork.CustomerLotRepository.GetAllAsync(condition: x => x.LotId == lotId);
                if (lots.Count() > 0)
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
                var customerLots = await _unitOfWork.CustomerLotRepository.GetAllAsync(condition: x => x.LotId == lotId && x.CustomerId == customerId);
                if (customerLots.Count() > 0)
                {
                    reponse.Code = 200;
                    var mapper = _mapper.Map<CheckCustomerInLotDTO>(customerLots.FirstOrDefault());
                    mapper.Result = true;
                    reponse.Data = mapper;
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

        public async Task<APIResponseModel> UpdateLotRange(int auctionId, string status)
        {
            var response = new APIResponseModel();
            try
            {
                var lots = await _unitOfWork.LotRepository.GetAllAsync(condition: x => x.AuctionId == auctionId);
                if (lots != null && lots.Any())
                {
                    foreach (var lot in lots)
                    {
                        lot.Status = status;
                    }

                    //cho lên redis update staus lot 1 loạt
                    _cacheService.UpdateMultipleLotsStatus(lots, status);


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

        public Task<APIResponseModel> CheckEndLot()
        {
            throw new NotImplementedException();
        }

        private async Task AddBidPriceIfApplicable(string lotType, PlaceBidFixedPriceAndSercet model)
        {
            var bidPrice = new BidPrice
            {
                CustomerId = (int)model.CustomerId,
                LotId = (int)model.LotId,
                CurrentPrice = model.CurrentPrice,
                BidTime = DateTime.UtcNow
            };

            if (lotType == EnumLotType.Fixed_Price.ToString() || lotType == EnumLotType.Secret_Auction.ToString())
            {
                await _unitOfWork.BidPriceRepository.AddAsync(bidPrice);
            }
        }

        private async Task<CustomerLot> checkCustomerRegisteredToLot(int customerId, int lotId)
        {
            var registered = await _unitOfWork.CustomerLotRepository.GetAllAsync(x => x.CustomerId == customerId && x.LotId == lotId);
            if (registered.FirstOrDefault() != null)
            {
                return registered.FirstOrDefault();
            }
            return null;
        }

        private async Task<bool> checkCustomerIntoBidPrice(int customerId, int lotId)
        {
            if (_unitOfWork.BidPriceRepository.GetAllAsync(x => x.CustomerId == customerId && x.LotId == lotId).Result.Any())
            {
                return true;
            }
            return false;
        }

        public async Task<APIResponseModel> CheckCustomerAuctioned(RequestCheckCustomerInLotDTO model)
        {
            var response = new APIResponseModel();
            try
            {
                var CustomerLotExist = await checkCustomerRegisteredToLot(model.CustomerId, model.LotId);
                if (CustomerLotExist != null)
                {
                    var bidPriceExist = CustomerLotExist?.Lot?.BidPrices?.FirstOrDefault(x => x.CustomerId == model.CustomerId && x.LotId == model.LotId);
                    if (bidPriceExist != null)
                    {
                        response.Code = 200;
                        response.IsSuccess = true;
                        response.Message = $"The customer is auctioned into the lot";
                        response.Data = _mapper.Map<BidPriceDTO>(bidPriceExist);
                        return response;
                    }
                    else
                    {
                        response.Code = 400;
                        response.IsSuccess = true;
                        response.Message = $"The customer haven't bid to the lot";
                        return response;
                    }
                }
                else
                {
                    response.Code = 400;
                    response.IsSuccess = true;
                    response.Message = $"Customer haven't register to lot.";

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

        public async Task<APIResponseModel> PlaceBidFixedPriceAndSercet(PlaceBidFixedPriceAndSercet model)
        {
            var response = new APIResponseModel();
            try
            {
                var playerJoined = await checkCustomerRegisteredToLot((int)model.CustomerId, (int)model.LotId);
                if (playerJoined == null)
                {
                    response.Code = 400;
                    response.IsSuccess = false;
                    response.Message = $"The customer is not register into the lot";
                    return response;
                }
                var bidPriceExist = playerJoined.Lot.BidPrices.FirstOrDefault(x => x.CustomerId == model.CustomerId && x.LotId == model.LotId);
                if (bidPriceExist != null)
                {
                    response.Code = 400;
                    response.IsSuccess = false;
                    response.Message = $"The customer is auctioned into the lot";
                    response.Data = _mapper.Map<BidPriceDTO>(bidPriceExist);
                    return response;
                }

                if (await checkCustomerIntoBidPrice((int)model.CustomerId, (int)model.LotId))
                {
                    response.Code = 400;
                    response.IsSuccess = false;
                    response.Message = $"The customer auctioned into the lot";
                    return response;
                }

                if (playerJoined.Lot.HaveFinancialProof == true)
                {
                    if (playerJoined.Customer.PriceLimit < model.CurrentPrice)
                    {
                        response.Code = 400;
                        response.IsSuccess = false;
                        response.Message = $"The customer haven't allow place bid out bidlimit current.";
                        return response;
                    }
                }

                var lot = await _unitOfWork.LotRepository.GetByIdAsync(model.LotId);
                if (lot != null)
                {
                    if (lot.Status != EnumStatusLot.Auctioning.ToString())
                    {
                        response.Code = 400;
                        response.IsSuccess = false;
                        response.Message = $"This Lot Is Not Auctioning , Can't Place Bid.";
                        return response;
                    }

                    var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(model.CustomerId);

                    if (model.CurrentPrice < lot.StartPrice || model.CurrentPrice > lot.FinalPriceSold && lot.LotType == EnumLotType.Secret_Auction.ToString())
                    {
                        response.Code = 400;
                        response.IsSuccess = false;
                        response.Message = $"The Price To Bid Must into the range of lot allow.";
                        return response;
                    }

                    if (lot.CurrentPrice < model.CurrentPrice || lot.CurrentPrice == null)
                    {
                        lot.CurrentPrice = model.CurrentPrice;

                        if (lot.LotType == EnumLotType.Secret_Auction.ToString())
                        {
                            foreach (var customerLot in lot.CustomerLots.Where(x => x.CustomerId != model.CustomerId))
                            {
                                customerLot.IsWinner = false;
                            }
                            lot.CustomerLots.First(x => x.CustomerId == model.CustomerId).IsWinner = true;
                        }
                    }

                    foreach (var customerLot in lot.CustomerLots.Where(x => x.CustomerId == model.CustomerId))
                    {
                        customerLot.CurrentPrice = model.CurrentPrice;
                    }
                    await AddBidPriceIfApplicable(lot.LotType, model);
                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        response.Code = 200;
                        response.IsSuccess = true;
                        response.Message = $"Place Bid Successfuly";
                    }
                    else
                    {
                        response.Code = 400;
                        response.IsSuccess = false;
                        response.Message = $"Place Bid Fail When Save";
                    }
                }
                else
                {
                    response.Code = 404;
                    response.IsSuccess = false;
                    response.Message = $"Not Found Lot";
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

        public async Task<APIResponseModel> PlaceBuyNow(PlaceBidBuyNowDTO placeBidBuyNowDTO)
        {
            var response = new APIResponseModel();
            try
            {
                var lot = await _unitOfWork.LotRepository.GetByIdAsync(placeBidBuyNowDTO.LotId);
                string lotGroupName = $"lot-{placeBidBuyNowDTO.LotId}";
                if (lot != null)
                {
                    if (lot.FinalPriceSold == null)
                    {
                        response.Code = 400;
                        response.IsSuccess = false;
                        response.Message = $"The lost haven't function for buy now.";
                        return response;
                    }

                    if (lot.LotType != EnumLotType.Public_Auction.ToString())
                    {
                        response.Code = 400;
                        response.IsSuccess = false;
                        response.Message = $"The lost isn't lot public cannot using buy now it.";
                        return response;
                    }

                    if (lot.Status != EnumStatusLot.Auctioning.ToString())
                    {
                        response.Code = 400;
                        response.IsSuccess = false;
                        response.Message = $"The lot is not currently up for auction, cannot be reserved now.";
                        return response;
                    }

                    var playerJoined = await checkCustomerRegisteredToLot((int)placeBidBuyNowDTO.CustomerId, (int)placeBidBuyNowDTO.LotId);

                    if (playerJoined == null)
                    {
                        response.Code = 400;
                        response.IsSuccess = false;
                        response.Message = $"The customer is not register into the lot";
                        return response;
                    }

                    if (playerJoined.Lot.HaveFinancialProof == true)
                    {
                        if (playerJoined.Customer.PriceLimit < playerJoined.Lot.FinalPriceSold)
                        {
                            response.Code = 400;
                            response.IsSuccess = false;
                            response.Message = $"The customer haven't allow place bid out bidlimit current.";
                            return response;
                        }
                    }

                    var bidData = new BiddingInputDTO
                    {
                        CurrentPrice = playerJoined.Lot.FinalPriceSold,
                        BidTime = DateTime.UtcNow

                    };
                    var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(placeBidBuyNowDTO.CustomerId);
                    var firstName = customer.FirstName;
                    var lastname = customer.LastName;


                    // Lưu dữ liệu đấu giá vào Redis stream
                    var bidPriceStream = _cacheService.AddToStream((int)placeBidBuyNowDTO.LotId, bidData, (int)placeBidBuyNowDTO.CustomerId);
                    await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPriceForStaff", bidPriceStream.CustomerId, firstName, lastname, bidPriceStream.CurrentPrice, bidPriceStream.BidTime);
                    await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPrice", bidPriceStream.CustomerId, bidPriceStream.CurrentPrice, bidPriceStream.BidTime);
                    await _hubContext.Clients.Group(lotGroupName).SendAsync("AuctionEndedWithWinnerPublic", "Phiên đã kết thúc!", placeBidBuyNowDTO.CustomerId, playerJoined.Lot.FinalPriceSold);

                    lot.Status = EnumStatusLot.Sold.ToString();
                    lot.CurrentPrice = lot.FinalPriceSold;
                    lot.ActualEndTime = DateTime.UtcNow;
                    _cacheService.UpdateLotStatus((int)placeBidBuyNowDTO.LotId, EnumStatusLot.Sold.ToString());
                    var winnerInLot = lot.CustomerLots?.First(x => x.CustomerId == placeBidBuyNowDTO.CustomerId
                                             && x.LotId == placeBidBuyNowDTO.LotId);

                    winnerInLot.Status = EnumCustomerLot.CreateInvoice.ToString();
                    winnerInLot.IsWinner = true;
                    winnerInLot.IsInvoiced = true;
                    winnerInLot.CurrentPrice = lot.FinalPriceSold;
                    foreach (var player in lot.CustomerLots.Where(x => x.CustomerId != placeBidBuyNowDTO.CustomerId
                                             && x.LotId == placeBidBuyNowDTO.LotId).ToList())
                    {
                        player.IsWinner = false;
                        player.IsRefunded = true;
                        player.Status = EnumCustomerLot.Refunded.ToString();
                    }

                    var bidPrice = new BidPrice
                    {
                        CustomerId = (int)placeBidBuyNowDTO.CustomerId,
                        LotId = (int)placeBidBuyNowDTO.LotId,
                        CurrentPrice = lot.FinalPriceSold,
                        BidTime = DateTime.UtcNow
                    };

                    var historyStatusCustomerLot = new HistoryStatusCustomerLot()
                    {
                        CustomerLotId = lot.CustomerLots.First(x => x.CustomerId == placeBidBuyNowDTO?.CustomerId).Id,
                        Status = EnumCustomerLot.CreateInvoice.ToString(),
                        CurrentTime = DateTime.UtcNow,
                    };

                    string redisKey = $"BidPrice:{lot.Id}";
                    var bidPricesLoser = _cacheService.GetSortedSetDataFilter<BidPrice>(redisKey, l => l.LotId == lot.Id);
                    await _unitOfWork.BidPriceRepository.AddRangeAsync(bidPricesLoser);
                    await _unitOfWork.HistoryStatusCustomerLotRepository.AddAsync(historyStatusCustomerLot);
                    await _unitOfWork.BidPriceRepository.AddAsync(bidPrice);
                    var totalprice = (float?)(winnerInLot.CurrentPrice + (winnerInLot.CurrentPrice * await _foorFeePercentService.GetPercentFloorFeeOfLot((float)winnerInLot.CurrentPrice)) - lot.Deposit);
                    var invoice = new Invoice
                    {

                        CustomerId = winnerInLot.CustomerId,
                        CustomerLotId = lot.CustomerLots.First(x => x.CustomerId == winnerInLot?.CustomerId).Id,
                        StaffId = lot.StaffId,
                        Price = winnerInLot.CurrentPrice,
                        Free = winnerInLot.CurrentPrice * await _foorFeePercentService.GetPercentFloorFeeOfLot((float)winnerInLot.CurrentPrice),
                        TotalPrice = totalprice,
                        CreationDate = DateTime.Now,
                        Status = EnumCustomerLot.CreateInvoice.ToString()
                    };
                    await _walletService.RefundToWalletForUsersAsync(lot.CustomerLots.Where(x => x.CustomerId != placeBidBuyNowDTO.CustomerId
                                             && x.LotId == placeBidBuyNowDTO.LotId).ToList());

                    await _unitOfWork.InvoiceRepository.AddAsync(invoice);
                    var notification = new ViewNotificationDTO
                    {
                        Title = $"Bidding win in Lot {placeBidBuyNowDTO.LotId}",
                        Description = $" You won in lot {placeBidBuyNowDTO.LotId} and system auto created invoice for you.",
                        Is_Read = false,
                        NotifiableId = invoice.Id,
                        AccountId = winnerInLot.Customer.AccountId,
                        CreationDate = DateTime.UtcNow,
                        Notifi_Type = "CreateInvoice",
                        ImageLink = lot.Jewelry.ImageJewelries.FirstOrDefault()?.ImageLink
                    };
                    var notificationEntity = _mapper.Map<Domain.Entity.Notification>(notification);
                    await _unitOfWork.NotificationRepository.AddAsync(notificationEntity);

                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        response.Code = 200;
                        response.IsSuccess = true;
                        response.Message = $"Place Bid Successfuly";
                    }
                    else
                    {
                        response.Code = 400;
                        response.IsSuccess = false;
                        response.Message = $"Place Bid Fail When Save";
                    }
                }
                else
                {
                    response.Code = 404;
                    response.IsSuccess = false;
                    response.Message = $"Not Found Lot";
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

        public async Task<APIResponseModel> TotalPlayerInLotFixed(int lotId)
        {
            var response = new APIResponseModel();
            try
            {
                var lotExist = await _unitOfWork.LotRepository.GetByIdAsync(lotId);
                if (lotExist != null)
                {
                    if (lotExist.LotType == EnumLotType.Fixed_Price.ToString())
                    {
                        response.Code = 200;
                        response.IsSuccess = true;
                        response.Data = lotExist.BidPrices?.Distinct().GroupBy(x => x.Customer).Count();
                        return response;
                    }
                    else
                    {
                        response.Code = 400;
                        response.IsSuccess = false;
                        response.Message = $"This lot isn't FixedLot";
                        return response;
                    }
                }
                else
                {
                    response.Code = 400;
                    response.IsSuccess = true;
                    response.Message = $"Not Found Lot";
                    return response;
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

        public async Task<APIResponseModel> GetPlayerInLotFixedAndSercet(int lotId)
        {
            var reponse = new APIResponseModel();
            var DTOs = new List<ViewPlayerInLotDTO>();
            try
            {
                var playersInLot = await _unitOfWork.CustomerLotRepository.GetAllAsync(x => x.LotId == lotId);
                if (playersInLot == null)
                {
                    reponse.Code = 404;
                    reponse.IsSuccess = true;
                    reponse.Message = "Current Time In Lot Haven't Player Register";
                }
                else
                {
                    foreach (var player in playersInLot)
                    {
                        var bidExist = await _unitOfWork.BidPriceRepository.GetAllAsync(x => x.CustomerId == player.CustomerId && x.LotId == player.LotId);
                        if (bidExist.Any())
                        {
                            var mapper = _mapper.Map<ViewPlayerInLotDTO>(player);
                            var bidTime = await _unitOfWork.BidPriceRepository.GetAllAsync(x => x.CurrentPrice == player.CurrentPrice && x.CustomerId == player.CustomerId && x.LotId == lotId);
                            mapper.BidTime = (bidTime.FirstOrDefault() != null) ? (DateTime?)bidTime.FirstOrDefault().CreationDate : null;
                            DTOs.Add(mapper);
                        }
                    }
                    reponse.Code = 200;
                    reponse.IsSuccess = true;
                    reponse.Message = $"Current Time In Lot Haven {playersInLot.Count} Player Register";
                    reponse.Data = DTOs;
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

        public async Task<APIResponseModel> UpdateLot(object lotDTO)
        {
            var reponse = new APIResponseModel();
            try
            {
                if (lotDTO == null)
                {
                    reponse.Code = 404;
                    reponse.IsSuccess = false;
                    reponse.Message = "Please check property of request.";
                }
                else
                {
                    var lot = _mapper.Map<Lot>(lotDTO);
                    var lotExist = await _unitOfWork.LotRepository.GetByIdAsync(lot.Id);
                    if (lotExist == null)
                    {
                        reponse.Code = 404;
                        reponse.IsSuccess = false;
                        reponse.Message = "Not Found Lot";
                        return reponse;
                    }
                    var autionexits = await _unitOfWork.AuctionRepository.GetByIdAsync(lotExist.AuctionId);
                    if (lotExist.Status != EnumStatusLot.Waiting.ToString() && ( autionexits.Status != EnumStatusAuction.UpComing.ToString()
                        && autionexits.Status != EnumStatusAuction.Waiting.ToString()))
                    {
                        reponse.Code = 404;
                        reponse.IsSuccess = false;
                        reponse.Message = "Current status can't update lot";
                        return reponse;
                    }
                    if (autionexits != null)
                    {

                        if (lotDTO is UpdateLotFixedPriceDTO fixedAuctionDTO)
                        {
                            _mapper.Map(fixedAuctionDTO,lotExist);
                            lotExist.LotType = EnumLotType.Fixed_Price.ToString();
                        }
                        if (lotDTO is UpdateLotSecretAuctionDTO secretAuctionDTO)
                        {
                            _mapper.Map(secretAuctionDTO, lotExist);
                            lotExist.LotType = EnumLotType.Secret_Auction.ToString();
                        }
                        if (lotDTO is UpdateLotPublicAuctionDTO publicAuctionDTO)
                        {
                            if (!publicAuctionDTO.IsHaveFinalPrice.Value)
                            {
                                lotExist.FinalPriceSold = null;
                            }
                            _mapper.Map(publicAuctionDTO, lotExist);
                            lotExist.LotType = EnumLotType.Public_Auction.ToString();
                        }
                        if (lotDTO is UpdateLotAuctionPriceGraduallyReducedDTO auctionPriceGraduallyReducedDTO)
                        {
                            _mapper.Map(auctionPriceGraduallyReducedDTO, lotExist);
                            lotExist.LotType = EnumLotType.Auction_Price_GraduallyReduced.ToString();
                        }
                        _unitOfWork.LotRepository.Update(lotExist);

                        if (await _unitOfWork.SaveChangeAsync() > 0)
                        {
                            // Lưu lot vào Redis(dung hash)
                            var lotRedis = new Lot
                            {
                                StartTime = lot.StartTime,
                                EndTime = lot.EndTime,
                                Id = lot.Id,
                                Status = lot.Status,
                                AuctionId = lot.AuctionId,
                                StartPrice = lot.StartPrice,
                                FinalPriceSold = lot.FinalPriceSold,
                                BidIncrement = lot.BidIncrement,
                                LotType = lot.LotType,
                                BidIncrementTime = lot.BidIncrementTime,
                                Title = lot.Title,
                                Deposit = lot.Deposit,
                                IsExtend = lot.IsExtend,
                                HaveFinancialProof = lot.HaveFinancialProof,
                                StaffId = lot.StaffId,
                                JewelryId = lot.JewelryId,
                                Round = lot.Round
                            };
                            _cacheService.SetLotInfo(lotRedis);

                            reponse.Code = 200;
                            reponse.IsSuccess = true;
                            reponse.Message = $"Update {lot.LotType} is successfuly";
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
    }
}
