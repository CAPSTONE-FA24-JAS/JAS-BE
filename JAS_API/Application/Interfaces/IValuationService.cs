using Application.ServiceReponse;
using Application.ViewModels.ValuationDTOs;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IValuationService
    {
        public Task<APIResponseModel> ConsignAnItem(ConsignAnItemDTO consignAnItem);

        public Task<APIResponseModel> GetAllAsync();

        public Task<APIResponseModel> UpdateStatusAsync(int id, int staffId, string status);

        public Task<APIResponseModel> CreatePreliminaryValuationAsync(int id, string status, float preliminaryPrice);

        public Task<APIResponseModel> getPreliminaryValuationByIdAsync(int id);

        public Task<APIResponseModel> getPreliminaryValuationByStatusOfSellerAsync(int sellerId, string status);

        public Task<APIResponseModel> getPreliminaryValuationsOfSellerAsync(int sellerId);

        public Task<APIResponseModel> UpdateStatusBySellerAsync(int id, string status);

    }
}
