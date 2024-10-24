using Application.ServiceReponse;
using Application.ViewModels.InvoiceDTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IInvoiceService
    {
        public Task<APIResponseModel> getInvoicesByStatusForManger(int status, int?pageSize, int? pageIndex);
        public Task<APIResponseModel> AsignShipper(int invoiceId, int shipperId, int status);

        public Task<APIResponseModel> GetInvoiceByStatusOfShipper(int shipperId, int status, int? pageSize, int? pageIndex);

     //   public Task<APIResponseModel> UpdateImageRecivedJewelryByShipper(int invoiceId, IFormFile imageDelivery);

        public Task<APIResponseModel> UpdateSuccessfulDeliveryByShipper(SuccessfulDeliveryRequestDTO deliveryDTO);


        public Task<APIResponseModel> FinishInvoiceByManager(int invoiceId, int status);

        public Task<APIResponseModel> getInvoicesByStatusForCustomer(int customerId, int? status, int? pageSize, int? pageIndex);

        Task<APIResponseModel> GetInvoiceDetail(int Id);
        Task<APIResponseModel> UpdateAddressToshipForInvoice(UpdateAddressToShipInvoice model);

    }
}
