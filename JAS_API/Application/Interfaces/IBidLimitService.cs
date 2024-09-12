using Application.ServiceReponse;
using Application.ViewModels.BidLimitDTO;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBidLimitService
    {
        public Task<APIResponseModel> CreateNewBidLimit(CreateBidLimitDTO createBidLimitDTO);
        public Task<APIResponseModel> ViewListBidLimit();
        public Task<APIResponseModel> ViewBidLimitByAccount(int accountId);
        public Task<APIResponseModel> ViewBidLimitById(int Id);
        public Task<APIResponseModel> UpdateStatus(int statusValue, int Id);
        public Task<APIResponseModel> GetStatusBidLimt();
    }
}
