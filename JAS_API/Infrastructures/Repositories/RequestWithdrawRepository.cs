﻿using Application.Interfaces;
using Application.Repositories;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories
{
    public class RequestWithdrawRepository : GenericRepository<RequestWithdraw>, IRequestWithdrawRepository
    {
        public RequestWithdrawRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
        }
    }
}
