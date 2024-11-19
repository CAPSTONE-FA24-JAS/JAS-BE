﻿using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.BidPriceDTOs;
using Application.ViewModels.CustomerLotDTOs;
using Application.ViewModels.LiveBiddingDTOs;
using AutoMapper;
using Azure;
using Castle.Core.Resource;
using CloudinaryDotNet;
using Domain.Entity;
using Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Middlewares;
using static Google.Apis.Requests.BatchRequest;

namespace Application.Services
{
    public class BidPriceService : IBidPriceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHubContext<BiddingHub> _hubContext;
        private readonly ICacheService _cacheService;
        private readonly ShareDB _shared;
        private readonly ICustomerLotService _customerLotService;


        public BidPriceService(IUnitOfWork unitOfWork, IMapper mapper, IHubContext<BiddingHub> hubContext,
                               ICacheService cacheService, ShareDB shareDB, ICustomerLotService customerLotService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hubContext = hubContext;
            _cacheService = cacheService;
            _shared = shareDB;
            _customerLotService = customerLotService;
            
        }

        public async Task<APIResponseModel> JoinBid(JoinLotRequestDTO request)
        {
            var reponse = new APIResponseModel();
            try
            {
                string lotGroupName = $"lot-{request.LotId}";
                string redisKey = $"BidPrice:{request.LotId}";
                var topBidders = _cacheService.GetSortedSetDataFilter<BidPriceDTO>(redisKey, l => l.LotId == request.LotId);
                var customerIds = topBidders.Select(b => b.CustomerId).Distinct().ToList();

                var customers = _unitOfWork.CustomerRepository.GetCustomersByIds(customerIds).ToDictionary(c => c.Id, c => new { c.FirstName, c.LastName });

                var topBidderWithName = topBidders.Select(x =>
                {
                    if (customers.TryGetValue(x.CustomerId, out var customer))
                    {
                        x.FirstName = customer.FirstName;
                        x.LastName = customer.LastName;
                    }
                    else
                    {
                        x.FirstName = "";
                        x.LastName = "";
                    }
                    return x;
                }).ToList();

                var lot = _cacheService.GetLotById(request.LotId);
              //  await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", request.LotId, lot.EndTime);

                

                    if (!_shared.connections.ContainsKey(request.ConnectionId))
                    {
                        _shared.connections[request.ConnectionId] = new AccountConnection
                        {
                            AccountId = request.AccountId,
                            LotId = request.LotId
                        };

                       
                        
                        await _hubContext.Groups.AddToGroupAsync(request.ConnectionId, lotGroupName);

                        
                        await _hubContext.Clients.Groups(lotGroupName).SendAsync("JoinLot", "admin", $"{request.AccountId} has joined lot {request.LotId}");
                        await _hubContext.Clients.Group(lotGroupName).SendAsync("SendCurrentPriceForReduceBidding", lot.CurrentPrice);

                    // await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", request.LotId, lot.EndTime);

                    if (topBidders.Any())
                         {
                              var highestBid = topBidders.FirstOrDefault();
                              await _hubContext.Clients.Group(lotGroupName).SendAsync("SendTopPrice", highestBid.CurrentPrice, highestBid.BidTime);
                              await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", request.LotId, lot.EndTime);
                              await _hubContext.Clients.Group(lotGroupName).SendAsync("SendHistoryBiddingOfLot", topBidders);
                              await _hubContext.Clients.Group(lotGroupName).SendAsync("SendHistoryBiddingOfLotOfStaff", topBidderWithName);

                         }
                         else
                         {

                             await _hubContext.Clients.Group(lotGroupName).SendAsync("SendTopPrice", 0, 0);
                             await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", request.LotId, lot.EndTime);
                             await _hubContext.Clients.Group(lotGroupName).SendAsync("SendHistoryBiddingOfLot", null);
                             await _hubContext.Clients.Group(lotGroupName).SendAsync("SendHistoryBiddingOfLotOfStaff", null);

                         }                     

                    }
                    else
                    {

                        await _hubContext.Clients.Groups(lotGroupName).SendAsync("JoinLot", "admin", $"{request.AccountId} has joined lot {request.LotId}");
                        if (topBidders.Any())
                        {

                            var highestBid = topBidders.FirstOrDefault();
                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendTopPrice", highestBid.CurrentPrice, highestBid.BidTime);
                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", request.LotId, lot.EndTime);
                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendHistoryBiddingOfLot", topBidders);
                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendHistoryBiddingOfLotOfStaff", topBidderWithName);


                        }
                        else
                        {
                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendTopPrice", 0, 0);
                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", request.LotId, lot.EndTime);
                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendHistoryBiddingOfLot", null);
                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendHistoryBiddingOfLotOfStaff", null);
                    }
                        
                    }
      
                reponse.IsSuccess = true;
                reponse.Message = "Join Bid successfully";
                reponse.Code = 200;
            }
            catch (Exception ex)
            {      
                    reponse.IsSuccess = false;
                    reponse.Message = ex.Message;
                    reponse.Code =500;
                
            }
            return reponse;
        }



        //public async Task<APIResponseModel> PlaceBiding(BiddingInputDTO request)
        //{
        //    var reponse = new APIResponseModel();

        //    try
        //    {
        //        if (_shared.connections.TryGetValue(request.ConnectionId, out AccountConnection conn))
        //        {
        //            var account = await _unitOfWork.AccountRepository.GetByIdAsync(conn.AccountId);
        //            var customerId = account.Customer.Id;
        //            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId);

        //            var lot = _cacheService.GetLotById(conn.LotId);
        //            string lotGroupName = $"lot-{conn.LotId}";
        //            var firstName = customer.FirstName;
        //            var lastname = customer.LastName;


        //                if (lot.HaveFinancialProof == true)
        //                {
        //                    var limitbid = customer.PriceLimit;
        //                    if (request.CurrentPrice > lot.BuyNowPrice)
        //                    {
        //                        request.CurrentPrice = lot.BuyNowPrice;
        //                    }
        //                    if (limitbid.HasValue && limitbid < request.CurrentPrice)
        //                    {
        //                        reponse.Message = "giá đặt cao hơn limit bid";
        //                        reponse.Code = 200;
        //                        reponse.IsSuccess = false;
        //                    }
        //                    else if (limitbid == null)
        //                    {
        //                        reponse.Message = "khong tim thay limit bid trong database";
        //                        reponse.Code = 200;
        //                        reponse.IsSuccess = false;
        //                    }
        //                    else
        //                    {


        //                            bool bidPlaced = _cacheService.PlaceBidWithLuaScript(conn.LotId, request, customerId);
        //                            if (!bidPlaced)
        //                            {
        //                                await _hubContext.Clients.Group(lotGroupName).SendAsync("SendResultCheckCurrentPrice", "Không được đặt giá thấp hơn hoặc bằng giá hiện tại", request.CurrentPrice);
        //                                reponse.IsSuccess = false;
        //                                reponse.Code = 200;
        //                                reponse.Message = "Đặt giá không thành công do không thỏa mãn điều kiện";
        //                            }
        //                            else
        //                            {
        //                                // Sau khi đặt giá thành công, xử lý và gửi thông báo
        //                                await ProcessBidPrice(request, lotGroupName, customerId, firstName, lastname, conn.LotId, lot);

        //                                reponse.IsSuccess = true;
        //                                reponse.Code = 200;
        //                                reponse.Message = "Đặt giá thành công!";
        //                            }

        //                    }                         
        //                }
        //                else
        //                {


        //                        bool bidPlaced = _cacheService.PlaceBidWithLuaScript(conn.LotId, request, customerId);
        //                        if (!bidPlaced)
        //                        {
        //                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendResultCheckCurrentPrice", "Không được đặt giá thấp hơn hoặc bằng giá hiện tại", request.CurrentPrice);
        //                            reponse.IsSuccess = false;
        //                            reponse.Code = 200;
        //                            reponse.Message = "Đặt giá không thành công do không thỏa mãn điều kiện";
        //                        }
        //                        else
        //                        {
        //                            // Sau khi đặt giá thành công, xử lý và gửi thông báo
        //                            await ProcessBidPrice(request, lotGroupName, customerId, firstName, lastname, conn.LotId, lot);

        //                            reponse.IsSuccess = true;
        //                            reponse.Code = 200;
        //                            reponse.Message = "Đặt giá thành công!";
        //                        }

        //                }

        //        }
        //        else
        //        {
        //            reponse.IsSuccess = false;
        //            reponse.Code = 404;
        //            reponse.Message = "Not found ConnectionId!";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        reponse.IsSuccess = false;
        //        reponse.Code = 500;
        //        reponse.Message = ex.Message;
        //    }
        //    return reponse;
        //}

        public async Task<APIResponseModel> PlaceBiding(BiddingInputDTO request)
        {
            var reponse = new APIResponseModel();

            try
            {
                if (_shared.connections.TryGetValue(request.ConnectionId, out AccountConnection conn))
                {
                    var account = await _unitOfWork.AccountRepository.GetByIdAsync(conn.AccountId);
                    var customerId = account.Customer.Id;
                    var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId);

                    var lot = _cacheService.GetLotById(conn.LotId);
                    string lotGroupName = $"lot-{conn.LotId}";
                    var firstName = customer.FirstName;
                    var lastname = customer.LastName;
                    string redisKey = $"BidPrice:{conn.LotId}";
                    var topBidders = _cacheService.GetSortedSetDataFilter<BidPrice>(redisKey, l => l.LotId == conn.LotId);
                    var highestBid = topBidders.FirstOrDefault();

                    if (lot.HaveFinancialProof == true)
                    {
                        var limitbid = customer.PriceLimit;
                        if (request.CurrentPrice > lot.BuyNowPrice)
                        {
                            request.CurrentPrice = lot.BuyNowPrice;
                        }
                        if (limitbid.HasValue && limitbid < request.CurrentPrice)
                        {
                            reponse.Message = "giá đặt cao hơn limit bid";
                            reponse.Code = 200;
                            reponse.IsSuccess = false;
                        }
                        else if (limitbid == null)
                        {
                            reponse.Message = "khong tim thay limit bid trong database";
                            reponse.Code = 200;
                            reponse.IsSuccess = false;
                        }
                        else
                        {                           
                            if(highestBid != null)
                            {
                                if (request.CurrentPrice < highestBid.CurrentPrice)
                                {
                                    await _hubContext.Clients.Group(lotGroupName).SendAsync("SendResultCheckCurrentPrice", "Không được đặt giá thấp hơn hoặc bằng giá hiện tại", request.CurrentPrice);
                                    reponse.IsSuccess = false;
                                    reponse.Code = 200;
                                    reponse.Message = "Đặt giá không thành công do không thỏa mãn điều kiện";
                                }
                                else
                                {
                                    var bidPrice =  _cacheService.AddToStream(conn.LotId, request, customerId);

                                    await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPriceForStaff", customerId, firstName, lastname, request.CurrentPrice, bidPrice.BidTime);
                                    await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPrice", customerId, request.CurrentPrice, bidPrice.BidTime);
                                    if (lot.EndTime.HasValue)
                                    {
                                        DateTime endTime = lot.EndTime.Value;

                                        //10s cuối
                                        TimeSpan extendTime = endTime - request.BidTime;

                                        // Nếu còn dưới 10 giây thì gia hạn thêm 10 giây
                                        if (extendTime.TotalSeconds < 10)
                                        {
                                            endTime = endTime.AddSeconds(10);
                                            _cacheService.UpdateLotEndTime(conn.LotId, endTime);
                                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", conn.LotId, endTime);
                                        }
                                        else
                                        {
                                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", conn.LotId, lot.EndTime);
                                        }
                                    }
                                    reponse.IsSuccess = true;
                                    reponse.Code = 200;
                                    reponse.Message = "Đã thêm giá vào hàng đợi stream";

                                }
                            }
                            else
                            {

                                _cacheService.AddToStream(conn.LotId, request, customerId);
                                if (lot.EndTime.HasValue)
                                {
                                    DateTime endTime = lot.EndTime.Value;

                                    //10s cuối
                                    TimeSpan extendTime = endTime - request.BidTime;

                                    // Nếu còn dưới 10 giây thì gia hạn thêm 10 giây
                                    if (extendTime.TotalSeconds < 10)
                                    {
                                        endTime = endTime.AddSeconds(10);
                                        _cacheService.UpdateLotEndTime(conn.LotId, endTime);
                                        await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", conn.LotId, endTime);
                                    }
                                    else
                                    {
                                        await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", conn.LotId, lot.EndTime);
                                    }
                                }
                                reponse.IsSuccess = true;
                                reponse.Code = 200;
                                reponse.Message = "Đã thêm giá vào hàng đợi stream";
                            }
                           
                            //trar về name, giá ĐẤU, thời gian
                           
                        }
                    }
                    else
                    {
                        if (highestBid != null)
                        {
                            if (request.CurrentPrice < highestBid.CurrentPrice)
                            {
                                //await _hubContext.Clients.Group(lotGroupName).SendAsync("SendResultCheckCurrentPrice", "Không được đặt giá thấp hơn hoặc bằng giá hiện tại", request.CurrentPrice);
                                reponse.IsSuccess = false;
                                reponse.Code = 200;
                                reponse.Message = "Đặt giá không thành công do không thỏa mãn điều kiện";
                            }
                            else
                            {
                               var bidPrice =  _cacheService.AddToStream(conn.LotId, request, customerId);
                                await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPriceForStaff", customerId, firstName, lastname, request.CurrentPrice, bidPrice.BidTime);
                                await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPrice", customerId, request.CurrentPrice, bidPrice.BidTime);
                                reponse.IsSuccess = true;
                                reponse.Code = 200;
                                reponse.Message = "Đã thêm giá vào hàng đợi stream";
                            }
                        }
                        else
                        {
                            _cacheService.AddToStream(conn.LotId, request, customerId);
                            reponse.IsSuccess = true;
                            reponse.Code = 200;
                            reponse.Message = "Đã thêm giá vào hàng đợi stream";
                        }

                    }

                }
                else
                {
                    reponse.IsSuccess = false;
                    reponse.Code = 404;
                    reponse.Message = "Not found ConnectionId!";
                }
            }
            catch (Exception ex)
            {
                reponse.IsSuccess = false;
                reponse.Code = 500;
                reponse.Message = ex.Message;
            }
            return reponse;
        }



        private async Task ProcessBidPrice(BiddingInputDTO request, string lotGroupName, int customerId, string firstName, string lastName, int lotId, Lot lot)
        {
            string redisKey = $"BidPrice:{lotId}";
            var topBidders = _cacheService.GetSortedSetDataFilter<BidPrice>(redisKey, l => l.LotId == lotId);
            var highestBid = topBidders.FirstOrDefault();

            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendTopPrice", highestBid.CurrentPrice, highestBid.BidTime);
            //trar về name, giá ĐẤU, thời gian
            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPriceForStaff", customerId, firstName, lastName, request.CurrentPrice, request.BidTime);

            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPrice", customerId, request.CurrentPrice, request.BidTime);

            

            // Lấy thời gian kết thúc từ Redis

            if (lot.EndTime.HasValue)
            {
                DateTime endTime = lot.EndTime.Value;

                //10s cuối
                TimeSpan extendTime = endTime - request.BidTime;

                // Nếu còn dưới 10 giây thì gia hạn thêm 10 giây
                if (extendTime.TotalSeconds < 10)
                {
                    endTime = endTime.AddSeconds(10);
                    _cacheService.UpdateLotEndTime(lotId, endTime);
                    await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", lotId, endTime);
                }
                else
                {
                    await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", lotId, lot.EndTime);
                }
            }
        }

            public async Task<APIResponseModel> PlaceBidForReducedBidding(BiddingInputDTO request)
        {
            var reponse = new APIResponseModel();
            try
            {
                if (_shared.connections.TryGetValue(request.ConnectionId, out AccountConnection conn))
                {
                    string lotGroupName = $"lot-{conn.LotId}";
                   
                        var account = await _unitOfWork.AccountRepository.GetByIdAsync(conn.AccountId);
                        var customerId = account.Customer.Id;
                        var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId);
                        var limitbid = customer.PriceLimit;
                        var customerName = customer.LastName + " " + customer.FirstName;
                    string redisKey = $"BidPrice:{conn.LotId}";
                    var lot = _cacheService.GetLotById(conn.LotId);
                    if (lot.HaveFinancialProof == true)
                    {
                        if (limitbid.HasValue && limitbid < request.CurrentPrice)
                        {
                            reponse.Message = "giá đặt cao hơn limit bid";
                            reponse.Code = 200;
                            reponse.IsSuccess = true;
                        }
                        else if (limitbid == null)
                        {
                            reponse.Message = "khong tim thay limit bid trong database";
                            reponse.Code = 200;
                            reponse.IsSuccess = true;
                        }
                        else
                        {

                            var bidData = new BidPrice
                            {
                                CurrentPrice = request.CurrentPrice,
                                BidTime = request.BidTime,
                                CustomerId = customerId,
                                LotId = conn.LotId
                            };
                           
                            // Lưu dữ liệu đấu giá vào Redis
                            _cacheService.SetSortedSetData<BidPrice>(redisKey, bidData, request.CurrentPrice);


                            //trar về name, giá ĐẤU, thời gian
                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPriceForStaff", customerId, customerName, request.CurrentPrice, request.BidTime);

                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPrice", customerId, request.CurrentPrice, request.BidTime);


                            await CountCustomerBidded(conn.LotId);
                            reponse.IsSuccess = true;
                            reponse.Code = 200;
                            reponse.Message = "Place bid successfully!";
                        }
                    }
                    else
                    {
                        var bidData = new BidPrice
                        {
                            CurrentPrice = request.CurrentPrice,
                            BidTime = request.BidTime,
                            CustomerId = customerId,
                            LotId = conn.LotId
                        };

                        // Lưu dữ liệu đấu giá vào Redis
                        _cacheService.SetSortedSetData<BidPrice>(redisKey, bidData, request.CurrentPrice);


                        //trar về name, giá ĐẤU, thời gian
                        await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPriceForStaff", customerId, customerName, request.CurrentPrice, request.BidTime);

                        await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPrice", customerId, request.CurrentPrice, request.BidTime);

                        await CountCustomerBidded(conn.LotId);

                        reponse.IsSuccess = true;
                        reponse.Code = 200;
                        reponse.Message = "Place bid successfully!";
                    }
                        
                    }
                                  
                else
                {
                    reponse.IsSuccess = false;
                    reponse.Code = 404;
                    reponse.Message = "Not found ConnectionId!";
                }
            }
            catch (Exception ex)
            {
                reponse.IsSuccess = false;
                reponse.Code = 500;
                reponse.Message = ex.Message;
            }
            return reponse;
        }

        public async Task CountCustomerBidded(int lotId)
        {
            string redisKey = $"BidPrice:{lotId}";
            var bidPrices = _cacheService.GetSortedSetDataFilter<BidPrice>(redisKey, x => x.LotId == lotId);
            var amountCustomerBidded = bidPrices.Distinct().Count();
            await _hubContext.Clients.All.SendAsync("SendAmountCustomerBid", "So luong nguoi da dau gia la: ", amountCustomerBidded);
        }
        public async Task<APIResponseModel> UpdateStatusBid(int lotId, int? status)
        {
            var reponse = new APIResponseModel();
            try
            {
                string lotGroupName = $"lot-{lotId}";
                
                var statusTranfer = EnumHelper.GetEnums<EnumStatusLot>().FirstOrDefault( x =>x.Value == status ).Name;
                var lot = await _unitOfWork.LotRepository.GetByIdAsync(lotId);
                if(lot != null)
                {
                    lot.Status = statusTranfer;
                    lot.ActualEndTime = DateTime.UtcNow;

                    _unitOfWork.LotRepository.Update(lot);
                    await _unitOfWork.SaveChangeAsync();
                    await _hubContext.Clients.Groups(lotGroupName).SendAsync("Closed Bid!");

                    reponse.IsSuccess = true;
                    reponse.Message = "update status lot successfully";
                    reponse.Code = 200;
                    reponse.Data = statusTranfer;
                }
                else
                {
                    reponse.IsSuccess = false;
                    reponse.Code = 200;
                    reponse.Message = "Khong tim thay lot!";
                }             
            }
            catch (Exception ex)


            {
                reponse.IsSuccess = false;
                reponse.Message = ex.Message;
                reponse.Code = 500;

            }
            return reponse;
        }


    }
}
