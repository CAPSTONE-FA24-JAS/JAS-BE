using Application;
using Application.Interfaces;
using Application.Services;
using Application.ViewModels.CustomerLotDTOs;
using Domain.Entity;
using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Middlewares
{
    public class BiddingHub : Hub 
    {
        private readonly ShareDB _shared;
        private readonly ICacheService _cacheService;
        private readonly IUnitOfWork _unitOfWork;

        public BiddingHub(ShareDB shared, ICacheService cacheService, IUnitOfWork unitOfWork)
        {
            _shared = shared;
            _cacheService = cacheService;
            _unitOfWork = unitOfWork;

        }

        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceivedMessage", "admin", message);
        }


        public async Task CountCustomerBidded(int lotId)
        {
            string redisKey = $"BidPrice:{lotId}";
            var bidPrices = _cacheService.GetSortedSetDataFilter<BidPrice>(redisKey, x => x.LotId == lotId);
            var amountCustomerBidded = bidPrices.Distinct().Count();
            await Clients.All.SendAsync("SendAmountCustomerBid", "So luong nguoi da dau gia la: ", amountCustomerBidded);
        }

      

    }
}
