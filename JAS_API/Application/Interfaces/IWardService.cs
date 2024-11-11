using Application.ServiceReponse;
using Application.ViewModels.WardDTOs;


namespace Application.Interfaces
{
    public interface IWardService
    {
        public Task<APIResponseModel> CreateNewWard(CreateWardDTO createWardDTO);
        public Task<APIResponseModel> ViewListWard();
    }
}
