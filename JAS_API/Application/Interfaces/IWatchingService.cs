using Application.ServiceReponse;
using Application.ViewModels.WatchingDTOs;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IWatchingService
    {
        Task<APIResponseModel> GetWatchingByCustomer(int customerId);
        Task<APIResponseModel> RemoveWatching(int watchingId);
        Task<APIResponseModel> AddNewWatching(CreateWatchingDTO model);
        Task<APIResponseModel> checkIsWatchingJewelryOfCustomeṛ̣(CreateWatchingDTO model);
    }
}
