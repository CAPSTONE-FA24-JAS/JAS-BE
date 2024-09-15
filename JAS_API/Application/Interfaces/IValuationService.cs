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

        public Task<APIResponseModel> AssignStaffForValuationAsync(int id, int staffId, string? status);

        public Task<APIResponseModel> CreatePreliminaryValuationAsync(int id, string status, float preliminaryPrice);

        public Task<APIResponseModel> getPreliminaryValuationByIdAsync(int id);

        //seller xem all consign item, all dinh gia so bo
        public Task<APIResponseModel> getPreliminaryValuationByStatusOfSellerAsync(int sellerId, string? status);

        //staff xem all consign item, all dinh gia so bo
        public Task<APIResponseModel> getPreliminaryValuationsByStatusOfStaffAsync(int staffId, string? status);

        //dung chung cho ca staff, seller update status
        public Task<APIResponseModel> UpdateStatusForValuationsAsync(int id, string status);


    }
}
