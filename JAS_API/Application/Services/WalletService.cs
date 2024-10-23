using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.BidLimitDTOs;
using Application.ViewModels.WalletDTOs;
using AutoMapper;
using Azure;
using Domain.Entity;
using Domain.Enums;

namespace Application.Services
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWalletTransactionService _walletTransactionService;

        public WalletService(IUnitOfWork unitOfWork, IMapper mapper, IWalletTransactionService walletTransactionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _walletTransactionService = walletTransactionService;
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
                    if (walletexist.Balance < (decimal)depositPrice)
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
                var walletExits =  await CheckBalance(requestWithdrawDTO.WalletId);
                if (walletExits.Data is WalletDTO cs && walletExits.IsSuccess)
                {
                    if (cs.Balance < requestWithdrawDTO.Amount)
                    {
                        reponse.IsSuccess = false;
                        reponse.Code = 400;
                        reponse.Message = "The amount exceeds the current balance.";
                    }
                    else
                    {
                        var request = _mapper.Map<RequestWithdraw>(requestWithdrawDTO);
                        var trans = new WalletTransaction()
                        {
                            Amount = -requestWithdrawDTO.Amount,
                            transactionType = EnumTransactionType.WithDrawWallet.ToString(),
                            DocNo = requestWithdrawDTO.WalletId,
                            TransactionTime = DateTime.UtcNow,
                            Status = EnumStatusTransaction.Pending.ToString(),
                            WalletId = requestWithdrawDTO.WalletId,
                        };
                        
                        if (!await LockFundsForWithdrawal(requestWithdrawDTO.WalletId, (decimal)requestWithdrawDTO.Amount))
                        {
                            reponse.IsSuccess = false;
                            reponse.Code = 400;
                            reponse.Message = "Update ForWithdrawal Faild";
                        }
                        else
                        {
                            await _unitOfWork.RequestWithdrawRepository.AddAsync(request);
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
                    walletexist.AvailableBalance += amountMoney;
                    _unitOfWork.WalletRepository.Update(walletexist);
                    
                }
                else
                {
                    if(amountMoney > walletexist.Balance)
                    {
                        reponse.IsSuccess = false;
                        reponse.Code = 400;
                        reponse.Message = $"Balance of wallet have {walletexist.Balance}, it less than amount money you want to deduct";
                        return reponse;
                    }
                    walletexist.Balance -= amountMoney;
                    walletexist.AvailableBalance -= amountMoney;
                    _unitOfWork.WalletRepository.Update(walletexist);
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

        public async Task<APIResponseModel> ApproveRequestWithdraw(int transId)
        {
            var reponse = new APIResponseModel();
            try
            {
                var transExist = await _unitOfWork.WalletTransactionRepository.GetByIdAsync(transId);
                if (transExist == null)
                {
                    reponse.Code = 404;
                    reponse.Message = "Not Found Trans Withdraw In System";
                    reponse.IsSuccess = false;
                }
                else
                {
                    var requestexist = await _unitOfWork.RequestWithdrawRepository.GetByIdAsync(transExist.DocNo);
                    if (requestexist != null)
                    {
                        requestexist.Wallet.FrozenBalance -= (decimal)requestexist.Amount;
                        requestexist.Wallet.Balance -= (decimal)requestexist.Amount;
                        transExist.Status = EnumStatusTransaction.Completed.ToString();
                        if (await _unitOfWork.SaveChangeAsync() > 0)
                        {
                            reponse.IsSuccess = true;
                            reponse.Code = 200;
                            reponse.Message = "Approve Successfuly";
                        }
                        else
                        {
                            reponse.Code = 500;
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
    }
}
