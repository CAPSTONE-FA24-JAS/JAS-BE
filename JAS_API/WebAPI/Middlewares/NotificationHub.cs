using Application.Interfaces;
using Application.Services;
using Application;
using Domain.Entity;
using Microsoft.AspNetCore.SignalR;
using Application.ViewModels.NotificationDTOs;

namespace WebAPI.Middlewares
{
    public class NotificationHub : Hub
    {
        private readonly ShareDBForNotification _shared;
        private readonly ICacheService _cacheService;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationHub(ShareDBForNotification shared, ICacheService cacheService, IUnitOfWork unitOfWork)
        {
            _shared = shared;
            _cacheService = cacheService;
            _unitOfWork = unitOfWork;

        }

        public async Task SpecificChatRoom(AccountConnectionForNoti conn)
        {
           
            //add vào group theo connectionId
            await Groups.AddToGroupAsync(Context.ConnectionId, conn.AccountId.ToString());

            //lưu thông tin kết nối -connection
            _shared.connections[Context.ConnectionId] = conn;

          

        }



    }
}
