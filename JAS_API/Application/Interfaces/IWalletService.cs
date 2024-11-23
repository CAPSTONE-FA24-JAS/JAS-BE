using Application.ServiceReponse;
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
        Task<APIResponseModel> CheckWalletExist(int customerId, float depositPrice);
        Task<APIResponseModel> UpdateBanlance(int walletId, decimal amountMoney, bool isDeposit);
        Task<APIResponseModel> AddWallet(WalletTransaction walletTransaction);
        Task<APIResponseModel> CheckPasswordWallet(int walletId, string password);
        //
        Task<APIResponseModel> RequestWithdraw(RequestWithdrawDTO requestWithdrawDTO);
        Task<APIResponseModel> CancelRequestWithdrawByCustomer(int customerId, int requestId);
        Task<APIResponseModel> ProgressRequestWithdraw(int requestId);
        Task<APIResponseModel> RejectRequestWithdrawByStaff(int requestId);
        Task<APIResponseModel> ApproveRequestWithdraw(int requestId);
        //
        Task<APIResponseModel> GetListRequestWithdrawForManagerment();
        Task<APIResponseModel> GetListRequestWithdrawForCustomer(int customerId);
        Task<APIResponseModel> RefundToWalletForUsersAsync(List<CustomerLot> customerLots);

    }
}
