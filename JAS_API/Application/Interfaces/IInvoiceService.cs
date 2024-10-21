using Application.ServiceReponse;
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

    }
}
