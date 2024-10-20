using Application.ViewModels.CustomerLotDTOs;
using Application.ViewModels.LiveBiddingDTOs;
using Domain.Entity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ShareDB
    {
        private readonly ConcurrentDictionary<string, AccountConnection> _connections = new();

        public ConcurrentDictionary<string, AccountConnection> connections => _connections;
    }
}
