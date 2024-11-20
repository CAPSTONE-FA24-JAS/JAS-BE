using Application.ServiceReponse;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICustomerLotService
    {
        public Task<APIResponseModel> GetCustomerLotByCustomerAndLot(int customerId, int lotId);
        public Task<APIResponseModel> GetBidsOfCustomer(int? customerIId, int? status, int? pageIndex, int? pageSize);
        public Task<APIResponseModel> GetPastBidOfCustomer(int customerIId, List<int> status, int? pageIndex, int? pageSize);
        public Task<APIResponseModel> GetMyBidByCustomerLotId(int customerLotId);
        Task<(bool, float?)> CheckBidPriceTop(float priceFuture, AutoBid autoBid);
        Task<bool> CheckTimeAutoBid(int customerLotId);
        Task<APIResponseModel> UpdateAutoBidPrice(int customerLotId, float priceCurrent);
        Task<APIResponseModel> GetWinnerForLot(int lotid);
        void CreateHistoryCustomerLot(HistoryStatusCustomerLot model);

    }
}
