using Application.ServiceReponse;
using Application.ViewModels.InvoiceDTOs;
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

        public Task<APIResponseModel> UpdateSuccessfulDeliveryByShipper(SuccessfulDeliveryRequestDTO deliveryDTO);


        public Task<APIResponseModel> UpdateStatus(int invoiceId, int status);
    }
}
