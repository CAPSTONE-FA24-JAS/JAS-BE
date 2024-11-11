using Application.Services;
using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Middlewares
{
    public class BiddingHub : Hub 
    {
        private readonly ShareDB _shared;
        public BiddingHub(ShareDB shared)
        {
            _shared = shared;

        }

        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceivedMessage", "admin", message);
        }      
    }
}
