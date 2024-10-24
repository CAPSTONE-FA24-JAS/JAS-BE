using Application.ServiceReponse;
using Application.ViewModels.ArtistDTOs;
using Application.ViewModels.JewelryDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IJewelryService
    {
        public Task<APIResponseModel> CreateJewelryAsync(CreateFinalValuationDTO jewelryDTO);

        public Task<APIResponseModel> GetJewelryAsync(int? pageSize, int? pageIndex);

        public Task<APIResponseModel> GetJewelryNoLotAsync(int? pageSize, int? pageIndex);

        public Task<APIResponseModel> RequestFinalValuationForManagerAsync(RequestFinalValuationForManagerDTO requestDTO);

        public Task<APIResponseModel> UpdateStatusByManagerAsync(int jewelryId, int status);

        public Task<APIResponseModel> RequestOTPForAuthorizedBySellerAsync(int jewelryId, int sellerId);

        public Task<APIResponseModel> VerifyOTPForAuthorizedBySellerAsync(int jewelryId, int sellerId, string opt);

    }
}
