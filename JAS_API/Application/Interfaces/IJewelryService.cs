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

        public Task<APIResponseModel> GetJewelryByCategoryAsync(int categoryId, int? pageSize, int? pageIndex);

        public Task<APIResponseModel> GetJewelryByArtistAsync(int artistId, int? pageSize, int? pageIndex);

        public Task<APIResponseModel> UpdateJewelryAsync(UpdateJewelryDTO model);

        public Task<APIResponseModel> DeleteJewelryAsync(int jewelryId);

        public Task<APIResponseModel> GetJewelrysIsSoldOut();
        public Task<APIResponseModel> SearchJewelry(string input);

        Task<APIResponseModel> GetEnumColorsShape();
        Task<APIResponseModel> GetEnumColorsDiamond();
        Task<APIResponseModel> GetEnumShapes();
        Task<APIResponseModel> GetEnumClarities();
        Task<APIResponseModel> GetEnumCuts();
        Task<APIResponseModel> getJewelryByIdAsync(int id);

    }
}
