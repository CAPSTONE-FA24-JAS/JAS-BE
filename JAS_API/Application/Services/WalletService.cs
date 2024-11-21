using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.BidLimitDTOs;
using Application.ViewModels.LotDTOs;
using Application.ViewModels.NotificationDTOs;
using Application.ViewModels.WalletDTOs;
using AutoMapper;
using Azure;
using CloudinaryDotNet;
using Domain.Entity;
using Domain.Enums;
using static System.TimeZoneInfo;

namespace Application.Services
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly IClaimsService _claimsService;

        public WalletService(IUnitOfWork unitOfWork, IMapper mapper, IWalletTransactionService walletTransactionService, IClaimsService claimsService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _walletTransactionService = walletTransactionService;
            _claimsService = claimsService;
        }

        public async Task<APIResponseModel> AddWallet(WalletTransaction walletTransaction)
        {
            var reponse = new APIResponseModel();
            try
            {
                var transaction = await _walletTransactionService.CreateNewTransaction(walletTransaction);
                if (!transaction.IsSuccess)
                {
                    return transaction;
                }
                var wallet = await UpdateBanlance((int)walletTransaction.DocNo, (decimal)walletTransaction.Amount, true);
                if (!wallet.IsSuccess)
                {
                    return wallet;
                }
                reponse.IsSuccess = true;
                reponse.Code = 200;
                reponse.Message = "Received Successfuly";
            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.ErrorMessages = new List<string> { e.Message };
                
            }
            return reponse;
        }

        public async Task<APIResponseModel> CheckBalance(int walletId)
        {
            var reponse = new APIResponseModel();
            try
            {
                var wallet = await _unitOfWork.WalletRepository.GetByIdAsync(walletId, includes: x => x.Customer);
                if(wallet == null)
                {
                    reponse.Code = 404;
                    reponse.Message = "Not Found Wallet In System";
                    reponse.IsSuccess = false;
                }
                else
                {
                    reponse.IsSuccess = true;
                    reponse.Code = 200;
                    reponse.Message = "Received Successfuly";
                    reponse.Data = _mapper.Map<WalletDTO>(wallet);
                }
                
            }
            catch(Exception e)
            {
               reponse.IsSuccess = false;
               reponse.ErrorMessages = new List<string> { e.Message };
            }
            return reponse;
        }

        public async Task<APIResponseModel> CheckPasswordWallet(int walletId, string password)
        {
            var reponse = new APIResponseModel();
            try
            {
                var CheckPassword = await _unitOfWork.WalletRepository.GetByIdAsync(walletId, condition: x => x.Password == HashPassword.HashWithSHA256(password));
                if (CheckPassword == null)
                {
                    reponse.Code = 404;
                    reponse.Message = "Password is Faild, please check password again";
                    reponse.IsSuccess = false;
                    return reponse;
                }
                    reponse.IsSuccess = true;
                    reponse.Code = 200;
                    reponse.Message = "Received Successfuly, Password of wallet avaiable";
                
            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.ErrorMessages = new List<string> { e.Message };
            }
            return reponse;
        }

        public async Task<APIResponseModel> CheckWalletExist(int customerId, float depositPrice)
        {
            var reponse = new APIResponseModel();
            try
            {
                //kiem tra co the chua
                var walletexists = await _unitOfWork.WalletRepository.GetAllAsync(condition: x => x.CustomerId == customerId);
                var walletexist = walletexists.FirstOrDefault();
                if (walletexist == null)
                {
                    reponse.Code = 404;
                    reponse.Message = "Customer haven't a wallet, please activate wallet!";
                    reponse.IsSuccess = false;
                    return reponse;
                }
                else
                {
                    //kiem tra du so du khong
                    if (walletexist.AvailableBalance < (decimal)depositPrice)
                    {
                        reponse.Code = 404;
                        reponse.Message = "Customer insufficient balance, Please add money into your wallet!";
                        reponse.IsSuccess = false;
                        return reponse;
                    }
                }
                reponse.IsSuccess = true;
                reponse.Code = 200;
                reponse.Message = "Received Successfuly, Wallet avaiable";
                reponse.Data = walletexist;
            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.ErrorMessages = new List<string> { e.Message };
            }
            return reponse;
        }

        public Task<APIResponseModel> CheckWalletStatus(int walletId)
        {
            throw new NotImplementedException();
        }

        public async Task<APIResponseModel> CreateWallet(CreateWalletDTO createWalletDTO)
        {
            var reponse = new APIResponseModel();
            try
            {
                var wallet = _mapper.Map<Wallet>(createWalletDTO);
                if (wallet == null)
                {
                    reponse.Code = 400;
                    reponse.Message = "Wallet have property is null";
                    reponse.IsSuccess = false;
                }
                else
                {
                    wallet.Balance = 0;
                    wallet.Status = EnumStatusWallet.UnLock.ToString();
                    wallet.Password = HashPassword.HashWithSHA256(createWalletDTO.Password);
                    await _unitOfWork.WalletRepository.AddAsync(wallet);
                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        reponse.IsSuccess = true;
                        reponse.Code = 200;
                        reponse.Message = "Received Successfuly";
                        reponse.Data = _mapper.Map<WalletDTO>(wallet);
                    }
                    else
                    {
                        reponse.Code = 500;
                        reponse.Message = "Error when saving";
                        reponse.IsSuccess = false;
                    }
                }
            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.ErrorMessages = new List<string> { e.Message };
            }
            return reponse;
        }

        public Task<APIResponseModel> LockWallet(int walletId)
        {
            throw new NotImplementedException();
        }

        public async Task<APIResponseModel> RequestWithdraw(RequestWithdrawDTO requestWithdrawDTO)
        {
            var reponse = new APIResponseModel();
            try
            {
                var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(requestWithdrawDTO.CustomerId);
                if (customer == null || customer.CreditCard == null)
                {
                    reponse.IsSuccess = false;
                    reponse.Code = 400;
                    reponse.Message = "Customer Haven't Credit Card For Withdraw, Please Add New Credit Card";
                    return reponse;
                }
                var walletExits =  await CheckBalance(requestWithdrawDTO.WalletId);
                if (walletExits.Data is WalletDTO cs && walletExits.IsSuccess)
                {
                    if ((float)cs.AvailableBalance < requestWithdrawDTO.Amount)
                    {
                        reponse.IsSuccess = false;
                        reponse.Code = 400;
                        reponse.Message = "The amount exceeds the current balance.";
                    }
                    else
                    {
                        var request = _mapper.Map<RequestWithdraw>(requestWithdrawDTO);
                        await _unitOfWork.RequestWithdrawRepository.AddAsync(request);
                        await _unitOfWork.SaveChangeAsync();
                        var trans = new WalletTransaction()
                        {
                            Amount = -requestWithdrawDTO.Amount,
                            transactionType = EnumTransactionType.WithDrawWallet.ToString(),
                            DocNo = request.Id,
                            TransactionTime = DateTime.UtcNow,
                            Status = EnumStatusTransaction.Pending.ToString(),
                            WalletId = requestWithdrawDTO.WalletId,
                            transactionPerson = requestWithdrawDTO.CustomerId
                        };
                        
                        if (!await LockFundsForWithdrawal(requestWithdrawDTO.WalletId, (decimal)requestWithdrawDTO.Amount))
                        {
                            reponse.IsSuccess = false;
                            reponse.Code = 400;
                            reponse.Message = "Update ForWithdrawal Faild";
                        }
                        else
                        {
                            await _unitOfWork.WalletTransactionRepository.AddAsync(trans);
                            
                            if (await _unitOfWork.SaveChangeAsync() > 0)
                            {
                                trans.DocNo = request.Id;
                                var resultTrans = await _walletTransactionService.CreateNewTransaction(trans);
                                if (resultTrans.IsSuccess)
                                {
                                    reponse.Code = 200;
                                    reponse.Message = "Add SuccessFull";
                                    reponse.IsSuccess = true;
                                }
                                else
                                {
                                    reponse.Code = 500;
                                    reponse.Message = "Error when saving Transaction";
                                    reponse.IsSuccess = false;
                                }
                            }
                            else
                            {
                                reponse.Code = 500;
                                reponse.Message = "Error when saving Request Withdraw";
                                reponse.IsSuccess = false;
                            }
                            
                        }
                        
                    }
                }
                else
                {
                    reponse.Code = 404;
                    reponse.Message = "Not Found Wallet";
                    reponse.IsSuccess = false;
                }
            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.ErrorMessages = new List<string> { e.Message };
            }
            return reponse;
        }

        public Task<APIResponseModel> TransactionHistory(int walletId)
        {
            throw new NotImplementedException();
        }

        public Task<APIResponseModel> UnLockWallet(int walletId)
        {
            throw new NotImplementedException();
        }

        public async Task<APIResponseModel> UpdateBanlance(int walletId, decimal amountMoney, bool isAdd)
        {
            var reponse = new APIResponseModel();
            try
            {
                var walletexist = await _unitOfWork.WalletRepository.GetByIdAsync(walletId);
                if (isAdd)
                {
                    walletexist.Balance += amountMoney;
                    if (walletexist.AvailableBalance == null)
                    {
                        walletexist.AvailableBalance = 0;
                    }
                    walletexist.AvailableBalance += amountMoney;

                    _unitOfWork.WalletRepository.Update(walletexist);
                    
                }
                else
                {
                    if(amountMoney > walletexist.AvailableBalance)
                    {
                        reponse.IsSuccess = false;
                        reponse.Code = 400;
                        reponse.Message = $"Balance of wallet have {walletexist.Balance}, it less than amount money you want to deduct";
                        return reponse;
                    }
                    if (walletexist.AvailableBalance == 0)
                    {
                        reponse.IsSuccess = false;
                        reponse.Code = 400;
                        reponse.Message = $"Available banlance of wallet is 0 cann't do it";
                        return reponse;
                    }
                    else
                    {
                        walletexist.AvailableBalance -= amountMoney;
                        walletexist.Balance = walletexist.AvailableBalance + walletexist.FrozenBalance;
                        _unitOfWork.WalletRepository.Update(walletexist);
                    }
                }
                if (await _unitOfWork.SaveChangeAsync() > 0)
                {
                    reponse.IsSuccess = true;
                    reponse.Code = 200;
                    reponse.Message = $"{(isAdd ? "Add" : "Deduct")} Wallet Successfully";
                }
            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.ErrorMessages = new List<string> { e.Message };
                reponse.Message = "UpdateBanlance FAILD";
            }
            return reponse;
        }

        public async Task<bool> LockFundsForWithdrawal(int walletId, decimal amountMoney)
        {
            var walletexist = await _unitOfWork.WalletRepository.GetByIdAsync(walletId);

            if (walletexist == null)
            {
                throw new Exception("Wallet not found.");
            }

            if (walletexist.AvailableBalance < amountMoney)
            {
                throw new Exception("Not enough available balance to lock.");
            }

            walletexist.AvailableBalance -= amountMoney;
            walletexist.FrozenBalance = (walletexist.FrozenBalance ?? 0) + amountMoney;
            _unitOfWork.WalletRepository.Update(walletexist);

            var result = await _unitOfWork.SaveChangeAsync();

            if (result > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<APIResponseModel> ApproveRequestWithdraw(int requestId)
        {
            var reponse = new APIResponseModel();
            try
            {
                var transExist = await _unitOfWork.WalletTransactionRepository.GetAllAsync(x => x.DocNo == requestId && x.Status == EnumStatusTransaction.Pending.ToString() 
                                                                                               && x.transactionType == EnumTransactionType.WithDrawWallet.ToString());
                var thisTransExit = transExist.FirstOrDefault();
                if (!transExist.Any())
                {
                    reponse.Code = 404;
                    reponse.Message = "Not Found Trans Withdraw In System";
                    reponse.IsSuccess = false;
                }
                else
                {
                    var requestexist = await _unitOfWork.RequestWithdrawRepository.GetByIdAsync(thisTransExit?.DocNo);
                    if (requestexist != null)
                    {
                        var transOfCompany = new Transaction()
                        {
                            Amount = requestexist.Amount,
                            DocNo = requestexist.Id,
                            TransactionPerson = requestexist.Wallet.CustomerId,
                            TransactionTime = DateTime.Now,
                            TransactionType = EnumTransactionType.WithDrawWallet.ToString(),
                        };

                        requestexist.Wallet.FrozenBalance -= (decimal)requestexist.Amount;
                        requestexist.Wallet.Balance -= (decimal)requestexist.Amount;
                        thisTransExit.Status = EnumStatusTransaction.Completed.ToString();
                        await _unitOfWork.TransactionRepository.AddAsync(transOfCompany);
                        if (await _unitOfWork.SaveChangeAsync() > 0)
                        {
                            reponse.IsSuccess = true;
                            reponse.Code = 200;
                            reponse.Message = "Approve Successfuly";
                        }
                        else
                        {
                            reponse.Code = 400;
                            reponse.Message = "Error when saving";
                            reponse.IsSuccess = false;
                        }
                    }
                    else
                    {
                        reponse.IsSuccess = false;
                        reponse.Code = 400;
                        reponse.Message = "Not Found Request In System";
                    }
                }

            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.ErrorMessages = new List<string> { e.Message };
            }
            return reponse;
        }

        public async Task<APIResponseModel> RefundToWalletForUsersAsync(List<CustomerLot> customerLot) 
        {
            var reponse = new APIResponseModel();
            try
            {
                foreach (var loser in customerLot) 
                {
                    //thuc hien hoan vi
                    var walletOfUser = loser.Customer.Wallet;
                    walletOfUser.Balance += (decimal?)loser.Lot.Deposit;
                    walletOfUser.AvailableBalance += (decimal?)loser.Lot.Deposit;
                    //tao transaction
                    var transactionCompany = new Transaction()
                    {
                        DocNo = loser.Id,
                        Amount = loser.Lot.Deposit,
                        TransactionTime = DateTime.Now,
                        TransactionType = EnumTransactionType.RefundDeposit.ToString(),
                        TransactionPerson = loser.CustomerId,
                    };

                    var transactionWallet = new WalletTransaction()
                    {
                        DocNo = loser.Id,
                        Amount = loser.Lot.Deposit,
                        TransactionTime = DateTime.Now,
                        transactionType = EnumTransactionType.RefundDeposit.ToString(),
                        transactionPerson = (int)loser.CustomerId,
                        Status = EnumStatusTransaction.Completed.ToString(),
                        WalletId = loser.Customer.Wallet.Id
                    };
                    var historyStatusCustomerLot = new HistoryStatusCustomerLot()
                    {
                        CustomerLotId = loser.Id,
                        Status = EnumCustomerLot.Refunded.ToString(),
                        CurrentTime = DateTime.UtcNow,
                    };
                    var notification = new ViewNotificationDTO
                    {
                        Title = $"Đấu giá thua Lot {loser.LotId}",
                        Description = $" Bạn đã lose lot {loser.LotId} và hệ thống đã tự động hoan tien cho bạn",
                        Is_Read = false,
                        NotifiableId = loser.Id,
                        AccountId = loser.Customer.AccountId,
                        CreationDate = DateTime.UtcNow,
                        Notifi_Type = "CustomerLot",
                    };
                    var notificationEntity = _mapper.Map<Notification>(notification);
                    await _unitOfWork.NotificationRepository.AddAsync(notificationEntity);
                    await _unitOfWork.HistoryStatusCustomerLotRepository.AddAsync(historyStatusCustomerLot);
                    await _unitOfWork.TransactionRepository.AddAsync(transactionCompany);
                    await _unitOfWork.WalletTransactionRepository.AddAsync(transactionWallet);
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

        public async Task<APIResponseModel> GetListRequestWithdrawForManagerment()
        {
            var reponse = new APIResponseModel();
            try
            {
                var transWithdraws = await _unitOfWork.WalletTransactionRepository.GetAllAsync(x => x.Status == EnumStatusTransaction.Pending.ToString()
                                                                                                      && x.transactionType == EnumTransactionType.WithDrawWallet.ToString());
                var requests = new List<RequestWithdraw>();
                foreach (var trans in transWithdraws)
                {
                    var requestWithdraw = await _unitOfWork.RequestWithdrawRepository.GetByIdAsync(trans.DocNo);
                    requests.Add(requestWithdraw);
                }

                if (!requests.Any())
                {
                    reponse.Code = 200;
                    reponse.Message = "CurrentTime Haven't Request";
                    reponse.IsSuccess = false;
                    return reponse;
                }
                else
                {
                    reponse.IsSuccess = true;
                    reponse.Code = 200;
                    reponse.Message = "Received Successfuly";
                    reponse.Data = _mapper.Map<IEnumerable<ViewRequestWithdrawDTO>>(requests);
                }
            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.ErrorMessages = new List<string> { e.Message };
            }
            return reponse;
        }
    }
}
