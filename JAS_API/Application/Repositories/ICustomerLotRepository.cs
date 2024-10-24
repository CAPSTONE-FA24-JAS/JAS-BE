using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface ICustomerLotRepository : IGenericRepository<CustomerLot>
    {
        Task<CustomerLot> GetCustomerLotByCustomerAndLot(int? customerIId, int? lotId);

        Task<(IEnumerable<CustomerLot> data, int totalItems)> GetBidsOfCustomer(int? customerIId, string? status, int? pageIndex, int? pageSize);

        Task<(IEnumerable<CustomerLot> data, int totalItems)> GetPastBidOfCustomer(int customerIId, IEnumerable<string> status, int? pageIndex, int? pageSize);

    }
}
