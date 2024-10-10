using Application.ServiceReponse;
using Application.ViewModels.WalletDTOs;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IWalletService
    {
        Task<APIResponseModel> CreateWallet(CreateWalletDTO createWalletDTO);
        Task<APIResponseModel> CheckBalance(int walletId);
        Task<APIResponseModel> TransactionHistory(int walletId);
        Task<APIResponseModel> LockWallet(int walletId);
        Task<APIResponseModel> UnLockWallet(int walletId);
        Task<APIResponseModel> CheckWalletStatus (int walletId);
    }
}
