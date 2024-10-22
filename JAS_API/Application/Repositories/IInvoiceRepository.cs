using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        Task<(IEnumerable<Invoice> data, int totalItems)> getInvoicesByStatusForManger(string status, int? pageIndex, int? pageSize);

        Task<(IEnumerable<Invoice> data, int totalItems)> getInvoicesByStatusForCustomer(int customerId, string? status, int? pageIndex, int? pageSize);
    }
}
