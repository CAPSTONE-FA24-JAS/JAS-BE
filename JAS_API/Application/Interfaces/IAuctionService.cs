using Application.ServiceReponse;
using Application.ViewModels.AuctionDTOs;

namespace Application.Interfaces
{
    public interface IAuctionService
    {
        Task<APIResponseModel> CreateAuction(CreateAuctionDTO createAuctionDTO);
        Task<APIResponseModel> ViewAutions();
        Task<APIResponseModel> GetAuctionById(int Id);
        Task<APIResponseModel> UpdateAuction(UpdateAuctionDTO updateAuctionDTO);
        Task<APIResponseModel> DeleteSolfAuction(int Id);
        Task<APIResponseModel> GetStatusAuction();
        Task<APIResponseModel> GetAuctionByStatus(int valueId);
    }
}
