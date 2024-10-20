using Application.Interfaces;
using Application.ServiceReponse;
using Application.ViewModels.BidLimitDTOs;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Domain.Entity;
using Domain.Enums;

namespace Application.Services
{
    public class WalletTransactionService : IWalletTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WalletTransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<APIResponseModel> CreateNewTransaction(WalletTransaction walletTransaction)
        {
            var reponse = new APIResponseModel();
            try
            {
                await _unitOfWork.WalletTransactionRepository.AddAsync(walletTransaction);
                if (await _unitOfWork.SaveChangeAsync() > 0)
                {
                    reponse.Message = $"Create New Transaction  SuccessFull";
                    reponse.Code = 200;
                    reponse.IsSuccess = true;
                }
                else
                {
                    reponse.Message = $"Create New Transaction Fail";
                    reponse.Code = 400;
                    reponse.IsSuccess = false;
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

        public async Task<APIResponseModel> UpdateTransaction(string Id)
        {
            var reponse = new APIResponseModel();
            try
            {
                var transactionexits = await _unitOfWork.WalletTransactionRepository.GetAllAsync(x => x.transactionId == Id);
                var trans = transactionexits.FirstOrDefault();
                if (trans != null)
                {
                    trans.Status = "Completed";
                     _unitOfWork.WalletTransactionRepository.Update(trans);
                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        reponse.Message = $"Update Transaction  SuccessFull";
                        reponse.Code = 200;
                        reponse.Data = trans;
                        reponse.IsSuccess = true;
                    }
                    else 
                    {
                        reponse.Message = $"UpdateTransaction Fail";
                        reponse.Code = 400;
                        reponse.IsSuccess = false;
                    }
                }
                else
                {
                    reponse.Message = $"Not Found";
                    reponse.Code = 400;
                    reponse.IsSuccess = false;
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
    }
}
