using Application.Interfaces;
using Application.Repositories;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        private readonly AppDbContext _dbContext;
        public CustomerRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        )
            : base(context, timeService, claimsService)
        {
            _dbContext = context;
        }


        public List<Customer> GetCustomersByIds(List<int> customerIds)
        {
            var customers = _dbContext.Customers
                             .Where(c => customerIds.Contains(c.Id))
                             .ToList();
            if(customers == null)
            {
                throw new Exception("khong co customer nao duoc tim thay theo id");
            }
            else
            {
                return customers;
            }
        }
    }
}
