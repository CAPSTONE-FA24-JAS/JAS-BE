using Application.Interfaces;
using Application.Repositories;
using Domain.Entity;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories
{
    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        private readonly AppDbContext _dbContext;
        public InvoiceRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        )
            : base(context, timeService, claimsService)
        {
            _dbContext = context;
        }

        public async Task<(IEnumerable<Invoice> data, int totalItems)> getInvoicesByStatusForManger(string status, int? pageIndex, int? pageSize)
        {
            var invoices = _dbContext.Invoices.Include(x => x.CustomerLot)
                                              .Where(x => x.CustomerLot.Status == status);

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10; // Assuming a default pageSize of 10 if an invalid value is passed

                invoices = invoices.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            var products = await invoices.ToListAsync(); 

            var totalItems = products.Count;

            if (products != null && products.Any())
            {
                return (products, totalItems);
            }
            else
            {
                throw new Exception("Don't have any Product");
            }
        }

       public async Task<(IEnumerable<Invoice> data, int totalItems)> getInvoicesByStatusForCustomer(int customerId, string? status, int? pageIndex, int? pageSize)
        {
            IQueryable<Invoice> invoices;
            if (status == null)
            {
                invoices = _dbContext.Invoices.Where(x => x.CustomerId == customerId);
            }
            else
            {
                invoices = _dbContext.Invoices.Include(x => x.CustomerLot)
                                             .Where(x => x.CustomerLot.Status == status && x.CustomerId == customerId);
            }
            

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10; // Assuming a default pageSize of 10 if an invalid value is passed

                invoices = invoices.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            var products = await invoices.ToListAsync();

            var totalItems = products.Count;

            if (products != null && products.Any())
            {
                return (products, totalItems);
            }
            else
            {
                throw new Exception("Don't have any Product");
            }
        }
    }
}
