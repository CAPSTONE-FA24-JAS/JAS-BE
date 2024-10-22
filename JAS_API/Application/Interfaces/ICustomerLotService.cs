using Application.ServiceReponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICustomerLotService
    {
        public Task<APIResponseModel> GetCustomerLotByCustomerAndLot(int customerId, int lotId);

        public Task<APIResponseModel> GetBidsOfCustomer(int? customerIId, int? status, int? pageIndex, int? pageSize);
    }
}
