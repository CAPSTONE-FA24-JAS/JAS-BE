using Application.ServiceReponse;
using Application.ViewModels.BidLimitDTOs;
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
        public Task<APIResponseModel> ViewBidLimitByCustomer(int customerId);
        public Task<APIResponseModel> ViewBidLimitById(int Id);
        public Task<APIResponseModel> UpdateStatus(UpdateBidLimitDTO updateBidLimitDTO);
        public Task<APIResponseModel> GetStatusBidLimt();
        public Task<APIResponseModel> FilterBidLimtByStatus(int status);
    }
}
