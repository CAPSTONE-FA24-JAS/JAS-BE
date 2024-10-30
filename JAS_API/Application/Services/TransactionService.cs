using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.TransactionDTOs;
using AutoMapper;
using Domain.Entity;
using Domain.Enums;

namespace Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TransactionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<APIResponseModel> CreateNewTransaction(Transaction transaction)
        {
            var reponse = new APIResponseModel();
            try
            {
                await _unitOfWork.TransactionRepository.AddAsync(transaction);
                if (await _unitOfWork.SaveChangeAsync() > 0)
                {
                    reponse.Message = $"Create New Transaction SuccessFull";
                    reponse.Code = 200;
                    reponse.IsSuccess = true;
                    reponse.Data = transaction;
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

        public async Task<APIResponseModel> GetAllTransaction()
        {
            var reponse = new APIResponseModel();
            try
            {
                var trans = await _unitOfWork.TransactionRepository.GetAllAsync();
                if (trans.Count > 0)
                {
                    reponse.Message = $"Received List Transaction SuccessFull";
                    reponse.Code = 200;
                    reponse.IsSuccess = true;
                    reponse.Data = _mapper.Map<IEnumerable<ViewTransactionDTO>>(trans);
                }
                else
                {
                    reponse.Message = $"Received To List Transaction Faild, Not Found";
                    reponse.Code = 400;
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

        public async Task<APIResponseModel> GetAllTransactionByTransType(int transTypeId)
        {
            var reponse = new APIResponseModel();
            try
            {
                var transType = EnumHelper.GetEnums<EnumTransactionType>().FirstOrDefault(x => x.Value == transTypeId).Name.ToString();
                var trans = await _unitOfWork.TransactionRepository.GetAllAsync(x => x.TransactionType == transType);
                if (trans.Count > 0)
                {
                    reponse.Message = $"Received List Transaction SuccessFull";
                    reponse.Code = 200;
                    reponse.IsSuccess = true;
                    reponse.Data = _mapper.Map<IEnumerable<ViewTransactionDTO>>(trans);
                }
                else
                {
                    reponse.Message = $"Received To List Transaction Faild, Not Found";
                    reponse.Code = 400;
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

        public async Task<APIResponseModel> UpdateTransaction(int Id)
        {
            var reponse = new APIResponseModel();
            try
            {
                var trans = await _unitOfWork.TransactionRepository.GetByIdAsync(Id);
                if (trans != null)
                {
                    _unitOfWork.TransactionRepository.Update(trans);
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
