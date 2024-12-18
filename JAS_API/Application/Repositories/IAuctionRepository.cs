﻿using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IAuctionRepository : IGenericRepository<Auction>
    {
        List<Auction> GetAuctionsAsync(string status);
    }
}
