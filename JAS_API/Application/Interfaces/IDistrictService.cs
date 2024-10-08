using Application.ServiceReponse;
using Application.ViewModels.DistrictDTOs;

namespace Application.Interfaces
{
    public interface IDistrictService
    {
        public Task<APIResponseModel> CreateNewDistrict(CreateDistrictDTO createDistrictDTO);
        public Task<APIResponseModel> ViewListDistrict();
    }
}
