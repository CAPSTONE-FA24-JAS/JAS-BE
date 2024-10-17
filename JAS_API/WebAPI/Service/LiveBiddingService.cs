
using Application;
using Application.Interfaces;
using Application.Utils;
using Domain.Entity;
using Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using WebAPI.Middlewares;

namespace WebAPI.Service
{
    public class LiveBiddingService : ILiveBiddingService
    {
        private readonly ICacheService _cacheService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<BiddingHub> _hubContext;

        public LiveBiddingService(ICacheService cacheService, IUnitOfWork unitOfWork, IHubContext<BiddingHub> hubContext)
        {
            _cacheService = cacheService;
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }
        public async Task ChecKLotEndAsync()
        {
            var lotLiveBidding = _cacheService.GetHashLots(l => l.Status == "Auctioning");

            foreach(var lot in lotLiveBidding)
            {
                var endTime = lot.EndTime;
                if(DateTime.UtcNow > endTime)
                {

                }
            }
        }

        public async Task CheckLotStartAsync()
        {
            var lotWithStartTime = _cacheService.GetHashLots( l => l.Status == "Created");

            foreach(var lot in lotWithStartTime)
            {
                var startTime = lot.StartTime;
                if(startTime.HasValue && DateTime.UtcNow >= startTime.Value)
                {
                    // Nếu đã đến thời gian bắt đầu, chuyển trạng thái lot sang 'Auctioning'
                    await StartLot(lot.Id);
                }
            }
        }

        private async Task StartLot(int lotId)
        {
            var lot = await _unitOfWork.LotRepository.GetByIdAsync(lotId);
            if(lot == null)
            {
                throw new Exception("Not found Lot Id");
            }
            lot.Status = EnumHelper.GetEnums<EnumStatusLot>().FirstOrDefault(x => x.Value == 2).Name;
            _unitOfWork.LotRepository.Update(lot);
            await _unitOfWork.SaveChangeAsync();

            _cacheService.UpdateLotStatus(lotId, lot.Status);
        }

        private async Task EndLot(int lotId)
        {


            var lot = await _unitOfWork.LotRepository.GetByIdAsync(lotId);
            if( lot == null)
            {
                throw new Exception("Not found Lot Id");
            }
            //cap nhat trang thai lot vao db va redis
            lot.Status = EnumHelper.GetEnums<EnumStatusLot>().FirstOrDefault(x => x.Value == 3).Name;
            _unitOfWork.LotRepository.Update(lot);
            

            _cacheService.UpdateLotStatus(lotId, lot.Status);


            var bidPrices = _cacheService.GetSortedSetData<BidPrice>("BidPrice");
            await _unitOfWork.BidPriceRepository.AddRangeAsync(bidPrices);

            // Truy xuất dữ liệu giá đấu từ Redis để xác định người thắng
            var topBidders = _cacheService.GetSortedSetData<BidPrice>("BidPrice");
            var winner = topBidders.FirstOrDefault();
            if( winner == null )
            {

            }
            await _unitOfWork.SaveChangeAsync();

        }
    }
}
