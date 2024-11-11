using Application.Interfaces;
using Application.ServiceReponse;
using Application.ViewModels.BidLimitDTOs;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Domain.Entity;
using Domain.Enums;
using Application.Utils;
using AutoMapper;
using Application.ViewModels.TransactionDTOs;

namespace Application.Services
{
    public class WalletTransactionService : IWalletTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WalletTransactionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<APIResponseModel> CreateNewTransaction(WalletTransaction walletTransaction)
        {
            var reponse = new APIResponseModel();
            try
            {
                await _unitOfWork.WalletTransactionRepository.AddAsync(walletTransaction);
                //if (await _unitOfWork.SaveChangeAsync() > 0)
                //{
                //    reponse.Message = $"Create New Transaction  SuccessFull";
                //    reponse.Code = 200;
                    reponse.IsSuccess = true;
                //}
                //else
                //{
                //    reponse.Message = $"Create New Transaction Fail";
                //    reponse.Code = 400;
                //    reponse.IsSuccess = false;
                //}
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

        public async Task<APIResponseModel> FilterWalletTransactionsOfCustomerByTransType(int customerId, int transTypeId)
        {
            var reponse = new APIResponseModel();
            try
            {
                var transType = EnumHelper.GetEnums<EnumTransactionType>().Where(x => x.Value == transTypeId).Select(x => x.Name).First();
                var trans = await _unitOfWork.WalletTransactionRepository.GetAllAsync(x => x.transactionPerson == customerId && x.transactionType == transType && x.Status == EnumStatusTransaction.Completed.ToString());
                if (trans.Count > 0 || trans != null)
                {
                    reponse.Message = $"Received To Transaction  SuccessFull";
                    reponse.Code = 200;
                    reponse.IsSuccess = true;
                    reponse.Data = _mapper.Map<IEnumerable<ViewWalletTransactionDTO>>(trans);
                }
                else
                {
                    reponse.Message = $"Received To Transaction Fail";
                    reponse.Code = 404;
                    reponse.IsSuccess = true;
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

        public async Task<APIResponseModel> ViewWalletTransactionsByCustomerId(int customerId)
        {
            var reponse = new APIResponseModel();
            try
            {
                var trans = await _unitOfWork.WalletTransactionRepository.GetAllAsync(x => x.transactionPerson == customerId && x.Status == EnumStatusTransaction.Completed.ToString(), sort: x => x.TransactionTime, true);
                if (trans.Count > 0 || trans != null)
                {
                    reponse.Message = $"Received To Transaction  SuccessFull";
                    reponse.Code = 200;
                    reponse.IsSuccess = true;
                    reponse.Data = _mapper.Map<IEnumerable<ViewWalletTransactionDTO>>(trans);
                }
                else
                {
                    reponse.Message = $"Received To Transaction Fail";
                    reponse.Code = 404;
                    reponse.IsSuccess = true;
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

        public async Task<APIResponseModel> ViewTransactionType()
        {
            var reponse = new APIResponseModel();
            try
            {
                var EnumTrans = EnumHelper.GetEnums<EnumTransactionType>();
                if (EnumTrans.Count > 0)
                {
                    reponse.Message = $"Received To Enum Trans  SuccessFull";
                    reponse.Code = 200;
                    reponse.IsSuccess = true;
                    reponse.Data = EnumTrans;
                }
                else
                {
                    reponse.Message = $"Received To Enum Transaction Fail";
                    reponse.Code = 404;
                    reponse.IsSuccess = true;
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
