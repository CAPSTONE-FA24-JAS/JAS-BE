﻿using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        Task<(IEnumerable<Invoice> data, int totalItems)> getInvoicesByStatusForManger(int? pageIndex, int? pageSize);

        Task<(IEnumerable<Invoice> data, int totalItems)> getInvoicesByStatusForCustomer(int customerId, string? status, int? pageIndex, int? pageSize);

        Task<(IEnumerable<Invoice> data, int totalItems)> getInvoicesRecivedByShipper(int shipperId, int? pageIndex, int? pageSize);
        Task<(IEnumerable<Invoice> data, int totalItems)> getInvoicesDeliveringByShipper(int shipperId, int? pageIndex, int? pageSize);

        Task<(List<int?> shipperIds, List<int> invoiceCounts)> getShipperAndInvoices();
        Task<(IEnumerable<Invoice> data, int totalItems)> getInvoicesDeliveringByShipperToAssign(int? pageIndex, int? pageSize);
        Task<List<Invoice>?> GetInvoiceForTotalProfit();
        Task<List<Invoice>?> GetInvoiceForTotalProfitByTime(int month, int year);
        }
}
