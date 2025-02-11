﻿using Application.ServiceReponse;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IWalletTransactionService
    {
        Task<APIResponseModel> CreateNewTransaction(WalletTransaction walletTransaction);
        Task<APIResponseModel> UpdateTransaction(string Id);
        Task<APIResponseModel> ViewTransactionType();
        Task<APIResponseModel> ViewWalletTransactionsByCustomerId(int customerId);
        Task<APIResponseModel> FilterWalletTransactionsOfCustomerByTransType(int customerId, int transTypeId);

    }
}
