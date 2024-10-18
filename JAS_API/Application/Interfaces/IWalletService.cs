﻿using Application.ServiceReponse;
using Application.ViewModels.WalletDTOs;
using Domain.Entity;

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
        Task<APIResponseModel> CheckWalletExist(int customerId, float depositPrice);
        Task<APIResponseModel> UpdateBanlance(int walletId, decimal amountMoney, bool isDeposit);

    }
}
