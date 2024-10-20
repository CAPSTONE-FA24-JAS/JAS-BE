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
                //kiem tra co the chua
                var walletexist = await _unitOfWork.WalletRepository.GetByIdAsync(walletId);
                // nap tien
                if (isAdd)
                {
                    walletexist.Balance += amountMoney;
                    _unitOfWork.WalletRepository.Update(walletexist);
                    
                }
                else
                {
                    if(amountMoney > walletexist.Balance)
                    {
                        reponse.IsSuccess = false;
                        reponse.Code = 402;
                        reponse.Message = $"Balance of wallet have {walletexist.Balance}, it less than amount money you want to deduct";
                        return reponse;
                    }
                    walletexist.Balance -= amountMoney;
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
    }
}
