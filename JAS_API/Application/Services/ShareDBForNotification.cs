using Application.ViewModels.LiveBiddingDTOs;
using Application.ViewModels.NotificationDTOs;
using CloudinaryDotNet;
using Microsoft.Identity.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ShareDBForNotification
    {
        private readonly ConcurrentDictionary<string, AccountConnectionForNoti> _connections = new();
        public ConcurrentDictionary<string, AccountConnectionForNoti> connections => _connections;
    }
}
