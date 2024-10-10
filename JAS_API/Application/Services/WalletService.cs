using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.BidLimitDTOs;
using Application.ViewModels.WalletDTOs;
using AutoMapper;
using Domain.Entity;
using Domain.Enums;

namespace Application.Services
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WalletService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
    }
}
