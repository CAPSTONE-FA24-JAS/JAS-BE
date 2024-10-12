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

        public Task<APIResponseModel> GetAllAsync(int? pageSize, int? pageIndex);


        public Task<APIResponseModel> AssignStaffForValuationAsync(int id, int staffId, int status);

        public Task<APIResponseModel> RequestPreliminaryValuationAsync(int id, int status);

        public Task<APIResponseModel> GetRequestPreliminaryValuationAsync(int? pageSize, int? pageIndex);

        public Task<APIResponseModel> CreatePreliminaryValuationAsync(int id, int status, float EstimatePriceMin, float EstimatePriceMax, int appraiserId);

        public Task<APIResponseModel> getPreliminaryValuationByIdAsync(int id);

        //seller xem all consign item, all dinh gia so bo
        public Task<APIResponseModel> getPreliminaryValuationByStatusOfSellerAsync(int sellerId, int? status, int? pageSize, int? pageIndex);

        public Task<APIResponseModel> getPreliminaryValuationByStatusOfAppraiserAsync(int appraiserId, int? status, int? pageSize, int? pageIndex);

        //staff xem all consign item, all dinh gia so bo
        public Task<APIResponseModel> getPreliminaryValuationsByStatusOfStaffAsync(int staffId, int? status, int? pageSize, int? pageIndex);

        public Task<APIResponseModel> getPreliminaryValuationsOfStaffAsync(int staffId, int? pageSize, int? pageIndex);

        //dung chung cho ca staff, seller update status
        public Task<APIResponseModel> UpdateStatusForValuationsAsync(int id, int status);

        public Task<APIResponseModel> RejectForValuationsAsync(int id, int status, string reason);

        public Task<APIResponseModel> CreateRecieptAsync(int id, ReceiptDTO model);

    }
}
