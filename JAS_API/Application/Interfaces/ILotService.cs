using Application.ServiceReponse;
using Application.ViewModels.LotDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ILotService
    {
        Task<APIResponseModel> CreateLot(object lotDTO);
        Task<APIResponseModel> GetLotTypes();
        Task<APIResponseModel> GetLotTypeById(int lotTypeId);
        Task<APIResponseModel> GetLots();
        Task<APIResponseModel> GetLotById(int Id);
        Task<APIResponseModel> GetLotByAuctionId(int auctionId);
        Task<APIResponseModel> GetListStatusOfLot();
        Task<APIResponseModel> RegisterToLot(RegisterToLotDTO registerToLotDTO);
        Task<APIResponseModel> GetCustomerLotByLot(int lotId);
        Task<APIResponseModel> CheckCustomerInLot(int customerId, int lotId);
        Task<APIResponseModel> UpdateLotRange(int auctionId, string status);
        Task<APIResponseModel> CheckEndLot();
        Task<APIResponseModel> PlaceBidFixedPriceAndSercet(PlaceBidFixedPriceAndSercet placeBidFixedPriceAndSercetDTO);
        Task<APIResponseModel> PlaceBuyNow(PlaceBidBuyNowDTO placeBidBuyNowDTO);
        Task<APIResponseModel> CheckCustomerAuctioned(RequestCheckCustomerInLotDTO model);
    }
}
