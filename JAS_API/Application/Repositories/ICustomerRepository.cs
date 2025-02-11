﻿using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        List<Customer> GetCustomersByIds(List<int> customerIds);
    }
}
