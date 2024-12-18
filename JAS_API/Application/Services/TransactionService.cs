﻿using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.TransactionDTOs;
using AutoMapper;
using Domain.Entity;
using Domain.Enums;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;

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
                var invoiceBankTrans = await _unitOfWork.InvoiceRepository.GetByIdAsync(transaction.DocNo, x => x.Status == EnumCustomerLot.PendingPayment.ToString());

                if(invoiceBankTrans != null)
                {
                    invoiceBankTrans.Status = EnumCustomerLot.Paid.ToString();
                }
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
                List<ViewTransactionDTO> listDTO = new List<ViewTransactionDTO>();
                var trans = await _unitOfWork.TransactionRepository.GetAllAsync();
                if (trans.Count > 0)
                {
                    foreach (var item in trans)
                    {
                        if (item.TransactionPerson.HasValue)
                        {
                            var mapper = _mapper.Map<ViewTransactionDTO>(item);
                            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(item.TransactionPerson);
                            mapper.CustomerName = customer.FirstName + " " + customer.LastName;
                            listDTO.Add(mapper);
                        }
                    }
                    reponse.Message = $"Received List Transaction SuccessFull";
                    reponse.Code = 200;
                    reponse.IsSuccess = true;
                    reponse.Data =listDTO;
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
                List<ViewTransactionDTO> listDTO = new List<ViewTransactionDTO>();
                var transType = EnumHelper.GetEnums<EnumTransactionType>().FirstOrDefault(x => x.Value == transTypeId).Name.ToString();
                var trans = await _unitOfWork.TransactionRepository.GetAllAsync(x => x.TransactionType == transType);
                if (trans.Count > 0)
                {
                    foreach (var item in trans)
                    {
                        if (item.TransactionPerson.HasValue)
                        {
                            var mapper = _mapper.Map<ViewTransactionDTO>(item);
                            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(item.TransactionPerson);
                            mapper.CustomerName = customer.FirstName + " " + customer.LastName;
                            listDTO.Add(mapper);
                        }
                    }
                    reponse.Message = $"Received List Transaction SuccessFull";
                    reponse.Code = 200;
                    reponse.IsSuccess = true;
                    reponse.Data = listDTO;
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

        public async Task<APIResponseModel> TotalProfit()
        {
            var response = new APIResponseModel();
            try
            {
                //var totalRevenue = await _unitOfWork.TransactionRepository.GetAllAsync(x => (x.TransactionType == EnumTransactionType.Banktransfer.ToString()
                //                                                      || x.TransactionType == EnumTransactionType.BuyPay.ToString()
                //                                                      || x.TransactionType == EnumTransactionType.AddWallet.ToString()));

                //var totalIncidentalCost = await _unitOfWork.TransactionRepository.GetAllAsync(x => (x.TransactionType == EnumTransactionType.RefundDeposit.ToString()
                //                                                      || x.TransactionType == EnumTransactionType.SellerPay.ToString()
                //                                                      || x.TransactionType == EnumTransactionType.WithDrawWallet.ToString()));
                //var totalProfit = totalRevenue.Sum(x => x.Amount) - totalIncidentalCost.Sum(x => x.Amount); 

                var invoices = await _unitOfWork.InvoiceRepository.GetInvoiceForTotalProfit();
                if (invoices == null)
                {
                    response.Code = 404;
                    response.IsSuccess = true;
                    response.Data = 0;
                    response.Message = $"Not Found Invoice.";
                }
                else
                {
                    var totalProfit = invoices.Sum(x => (x.TotalPrice ?? 0 - x.Free ?? 0));
                    response.Code = 200;
                    response.Data = totalProfit;
                    response.IsSuccess = true;
                    response.Message = $"Received Successfully Total Profit: {totalProfit}.";
                }
               
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.IsSuccess = false;
                response.Message = $"Exception When System Processcing";
            }
            return response;
        }

        public async Task<APIResponseModel> TotalProfitByMonth(int month, int year)
        {
            var response = new APIResponseModel();
            try
            {
                //var totalRevenue = await _unitOfWork.TransactionRepository.GetAllAsync(x => x.CreationDate.Month == month && x.CreationDate.Year == year
                //                                                      && (x.TransactionType == EnumTransactionType.Banktransfer.ToString()
                //                                                      || x.TransactionType == EnumTransactionType.BuyPay.ToString()
                //                                                      || x.TransactionType == EnumTransactionType.AddWallet.ToString()));

                //var totalIncidentalCost = await _unitOfWork.TransactionRepository.GetAllAsync(x => x.CreationDate.Month == month && x.CreationDate.Year == year
                //                                                      && (x.TransactionType == EnumTransactionType.RefundDeposit.ToString()
                //                                                      || x.TransactionType == EnumTransactionType.SellerPay.ToString()
                //                                                      || x.TransactionType == EnumTransactionType.WithDrawWallet.ToString()));

                var invoices = await _unitOfWork.InvoiceRepository.GetInvoiceForTotalProfitByTime(month, year);
                if (invoices == null)
                {
                    response.Code = 404;
                    response.IsSuccess = true;
                    response.Data = 0;
                    response.Message = $"Not Found Invoice.";
                }
                else
                {
                    var totalProfit = invoices.Sum(x => (x.TotalPrice ?? 0 - x.Free ?? 0));
                    response.Code = 200;
                    response.Data = totalProfit;
                    response.IsSuccess = true;
                    response.Message = $"Received Successfully Total Profit: {totalProfit}.";
                }
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.IsSuccess = false;
                response.Message = $"Exception When System Processcing";
            }
            return response;
        }
    }
}
