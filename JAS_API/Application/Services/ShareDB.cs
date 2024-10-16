using Application.ViewModels.CustomerLotDTOs;
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
        private readonly ConcurrentDictionary<string, CustomerLotDTO> _connections = new();

        public ConcurrentDictionary<string, CustomerLotDTO> connections => _connections;
    }
}
