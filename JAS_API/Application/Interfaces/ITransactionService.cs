﻿using Application.ServiceReponse;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITransactionService
    {
        Task<APIResponseModel> CreateNewTransaction(Transaction transaction);
        Task<APIResponseModel> UpdateTransaction(int Id);
    }
}
