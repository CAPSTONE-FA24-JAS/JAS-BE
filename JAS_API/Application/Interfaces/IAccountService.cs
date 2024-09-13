using Application.ServiceReponse;
using Application.ViewModels.AccountDTOs;
namespace Application.Interfaces
{
    public interface IAccountService
    {
        public Task<APIResponseModel> ViewListAccounts();
        public Task<APIResponseModel> SearchAccountByName(string Name);
        public Task<APIResponseModel> GetProfileAccount(int Id);
        public Task<APIResponseModel> CreateAccount(CreateAccountDTO createDTO);
        public Task<APIResponseModel> UpdateProfile(int Id, UpdateProfileDTO updateDTO);
        public Task<APIResponseModel> BanAccount(int Id);
        public Task<APIResponseModel> UnBanAccount(int Id);
        public Task<APIResponseModel> DeleteAccount(int Id);
        public Task<APIResponseModel> GetFilter();
        public Task<APIResponseModel> FilterAccount(int filter);
        public Task<APIResponseModel> FilterAccountByRole(int role);


    }
}
