﻿using Application.ServiceReponse;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IFoorFeePercentService
    {
        Task<float?> GetPercentFloorFeeOfLot(float currentPrice);

        Task<APIResponseModel> GetFloorFeesAsync();
    }
}
