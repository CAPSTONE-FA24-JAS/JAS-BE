﻿using Application.ViewModels.JewelryDTOs;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IJewelryRepository : IGenericRepository<Jewelry>
    {
        Task<IEnumerable<Jewelry>> GetAllAynsc(int? pageIndex = null, int? pageSize = null);
    }
}
