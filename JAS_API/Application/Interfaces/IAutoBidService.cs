using Application.ServiceReponse;
using Application.ViewModels.AutoBidDTOs;

namespace Application.Interfaces
{
    public interface IAutoBidService
    {
        public Task<APIResponseModel> SetAutoBid(CreateAutoBidDTO createAutoBidDTO);
        public Task<APIResponseModel> GetAutoBisByCustomerdLot(int customerLotId);
    }
}
