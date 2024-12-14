using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.BidPriceDTOs;
using Application.ViewModels.CustomerLotDTOs;
using Application.ViewModels.InvoiceDTOs;
using Application.ViewModels.LiveBiddingDTOs;
using AutoMapper;
using Azure;
using Castle.Core.Resource;
using CloudinaryDotNet;
using Domain.Entity;
using Domain.Enums;
using iTextSharp.text.pdf.parser.clipper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IHubContext<NotificationHub> _notificationHub;


        public BidPriceService(IUnitOfWork unitOfWork, IMapper mapper, IHubContext<BiddingHub> hubContext,
                               ICacheService cacheService, ShareDB shareDB, ICustomerLotService customerLotService, IHubContext<NotificationHub> notificationHub)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hubContext = hubContext;
            _cacheService = cacheService;
            _shared = shareDB;
            _customerLotService = customerLotService;
            _notificationHub = notificationHub;
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

                        
                        await _hubContext.Clients.Group(lotGroupName).SendAsync("JoinLot", "admin", $"{request.AccountId} has joined lot {request.LotId}");
                        await _hubContext.Clients.Group(lotGroupName).SendAsync("SendCurrentPriceForReduceBidding", lot.CurrentPrice);
                        await _hubContext.Clients.Group(lotGroupName).SendAsync("StatusBid", lot.Status);
                        await CountCustomerBidded(request.LotId);
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
                  //  var highestBid = _cacheService.GetHighestPrice<BidPrice>(redisKey);
                  //  var topBidders = _cacheService.GetSortedSetDataFilter<BidPrice>(redisKey, l => l.LotId == conn.LotId);
                  //  var highestBid = topBidders.FirstOrDefault();

                    if(lot.FinalPriceSold != null)
                    {
                        if (request.CurrentPrice > lot.FinalPriceSold)
                        {
                            request.CurrentPrice = lot.FinalPriceSold;
                        }
                    }                 
                    //neu co chungh minh tai chinh
                    if (lot.HaveFinancialProof == true)
                    {
                        var limitbid = customer.PriceLimit;
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
                                    var bidPrice =  _cacheService.AddToStream(conn.LotId, request, customerId);

                                    await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPriceForStaff", customerId, firstName, lastname, request.CurrentPrice, bidPrice.BidTime);
                                    await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPrice", customerId, request.CurrentPrice, bidPrice.BidTime);

                                    if (lot.IsExtend == true)
                                    {
                                        if (lot.EndTime.HasValue)
                                        {
                                            DateTime endTime = lot.EndTime.Value;

                                            //10s cuối
                                            TimeSpan extendTime = endTime - request.BidTime;

                                            //neu round vo tan
                                            if (lot.Round == null)
                                            {
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
                                            else
                                            {
                                                if (extendTime.TotalSeconds < 10)
                                                {
                                                    if (lot.Round == 0)
                                                    {
                                                        await _hubContext.Clients.Group(lotGroupName).SendAsync("HetRound");
                                                        await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", conn.LotId, lot.EndTime);

                                                    }
                                                    else
                                                    {
                                                        endTime = endTime.AddSeconds(10);
                                                        var round = (int)lot.Round - 1;
                                                        _cacheService.UpdateLotEndTime(conn.LotId, endTime);
                                                        _cacheService.UpdateLotRound(conn.LotId, round);
                                                        await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", conn.LotId, endTime);
                                                    }

                                                }
                                                else
                                                {
                                                    await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", conn.LotId, lot.EndTime);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (lot.EndTime.HasValue)
                                        {
                                            DateTime endTime = lot.EndTime.Value;
                                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", conn.LotId, endTime);
                                        }
                                    }
                                    reponse.IsSuccess = true;
                                    reponse.Code = 200;
                                    reponse.Message = "Đã thêm giá vào hàng đợi stream";                               
                            }                          
                    }
                    else
                    {                       
                               var bidPrice =  _cacheService.AddToStream(conn.LotId, request, customerId);
                                await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPriceForStaff", customerId, firstName, lastname, request.CurrentPrice, bidPrice.BidTime);
                                await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPrice", customerId, request.CurrentPrice, bidPrice.BidTime);
                                if (lot.IsExtend == true)
                                {
                                    if (lot.EndTime.HasValue)
                                    {
                                        DateTime endTime = lot.EndTime.Value;

                                        //10s cuối
                                        TimeSpan extendTime = endTime - request.BidTime;
                                        if (lot.Round == null)
                                        {
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
                                        else
                                        {
                                            if (extendTime.TotalSeconds < 10)
                                            {
                                                if (lot.Round == 0)
                                                {

                                                    await _hubContext.Clients.Group(lotGroupName).SendAsync("HetRound");
                                                    await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", conn.LotId, lot.EndTime);

                                                }
                                                else
                                                {
                                                    endTime = endTime.AddSeconds(10);
                                                    var round = (int)lot.Round - 1;
                                                    _cacheService.UpdateLotEndTime(conn.LotId, endTime);
                                                    _cacheService.UpdateLotRound(conn.LotId, round);
                                                    await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", conn.LotId, endTime);
                                                }

                                            }
                                            else
                                            {
                                                await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", conn.LotId, lot.EndTime);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (lot.EndTime.HasValue)
                                    {
                                        DateTime endTime = lot.EndTime.Value;
                                        await _hubContext.Clients.Group(lotGroupName).SendAsync("SendEndTimeLot", conn.LotId, endTime);
                                    }
                                }                                    
                                reponse.IsSuccess = true;
                                reponse.Code = 200;
                                reponse.Message = "Đã thêm giá vào hàng đợi stream";
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
                            await _unitOfWork.BidPriceRepository.AddAsync(bidData);
                            await _unitOfWork.SaveChangeAsync();

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
            if(bidPrices != null)
            {
                var amountCustomerBidded = bidPrices.Distinct().Count();
                await _hubContext.Clients.All.SendAsync("SendAmountCustomerBid", "So luong nguoi da dau gia la: ", amountCustomerBidded);
            }          
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
                    _cacheService.UpdateLotStatus(lotId, statusTranfer);
                    await _hubContext.Clients.Group(lotGroupName).SendAsync("UpdateStatusBid", lot.Status);

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

        public async Task<APIResponseModel> cancelLot(int lotId)
        {
            var reponse = new APIResponseModel();
            try
            {
                string lotGroupName = $"lot-{lotId}";

                
                var lot = await _unitOfWork.LotRepository.GetByIdAsync(lotId);
                if (lot != null)
                {
                    lot.Status = EnumStatusLot.Cancelled.ToString();
                    lot.ActualEndTime = DateTime.UtcNow;

                    _unitOfWork.LotRepository.Update(lot);
                    await _unitOfWork.SaveChangeAsync();
                    _cacheService.UpdateLotStatus(lotId, EnumStatusLot.Cancelled.ToString());
                    _cacheService.UpdateLotActualEndTime(lotId, (DateTime)lot.ActualEndTime);
                    await _hubContext.Clients.Group(lotGroupName).SendAsync("UpdateStatusBid", lot.Status);
                    await _hubContext.Clients.Group(lotGroupName).SendAsync("CanceledAuctionPublic", "Phiên đã bi huy!");
                    await HandleLoserLot(lotId);
                    reponse.IsSuccess = true;
                    reponse.Message = "update status lot successfully";
                    reponse.Code = 200;
                    reponse.Data = lot.Status;
                }
                else
                {
                    reponse.IsSuccess = false;
                    reponse.Code = 404;
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


        private async Task HandleLoserLot(int lotId)
        {
                string redisKey = $"BidPrice:{lotId}";
                var bidPrices = _cacheService.GetSortedSetDataFilter<BidPrice>(redisKey, l => l.LotId == lotId);
                await _unitOfWork.BidPriceRepository.AddRangeAsync(bidPrices);
                var lot = await _unitOfWork.LotRepository.GetByIdAsync(lotId);

                //lay ra list customerLot theo  lotId 
                var customerlots = await _unitOfWork.CustomerLotRepository.GetAllAsync(x => x.LotId == lotId);
                if (customerlots != null)
                {
                    List<CustomerLot> listCustomerLot = new List<CustomerLot>();
                    foreach (var customerLot in customerlots)
                    {
                       customerLot.IsWinner = false;

                        //hoan coc cho loser
                        var walletOfLoser = await _unitOfWork.WalletRepository.GetByCustomerId(customerLot.CustomerId);
                        walletOfLoser.AvailableBalance += ((decimal?)customerLot?.Lot?.Deposit ?? 0);
                        walletOfLoser.Balance = walletOfLoser.AvailableBalance ?? 0 + walletOfLoser.FrozenBalance ?? 0;
                        _unitOfWork.WalletRepository.Update(walletOfLoser);
                       customerLot.IsRefunded = true;
                       customerLot.Status = EnumCustomerLot.Refunded.ToString();
                        listCustomerLot.Add(customerLot);

                        var maxBidPriceLoser = await _unitOfWork.BidPriceRepository.GetMaxBidPriceByCustomerIdAndLot(customerLot.CustomerId, lotId);
                        if (maxBidPriceLoser == null)
                        {
                           customerLot.CurrentPrice = 0;
                        }
                        else
                        {
                            customerLot.CurrentPrice = maxBidPriceLoser.CurrentPrice;
                        }

                        _unitOfWork.CustomerLotRepository.Update(customerLot);
                        //lưu history của loser là refunded
                        var historyCustomerlotLoser = new HistoryStatusCustomerLot()
                        {
                            Status = customerLot.Status,
                            CustomerLotId = customerLot.Id,
                            CurrentTime = DateTime.UtcNow,
                        };
                        await _unitOfWork.HistoryStatusCustomerLotRepository.AddAsync(historyCustomerlotLoser);

                        //tao notification cho loser
                        var notificationloser = new Notification
                        {
                            Title = $"Lot {lotId} had been canceld",
                            Description = $" Lot {lotId} had been canceld and system auto refunded deposit for you",
                            Is_Read = false,
                            NotifiableId = customerLot.Id,  //cusrtomerLot => dẫn tới myBid
                            AccountId = customerLot.Customer.AccountId,
                            CreationDate = DateTime.UtcNow,
                            Notifi_Type = "Refunded",
                            ImageLink = lot.Jewelry.ImageJewelries.FirstOrDefault()?.ImageLink

                            //  ImageLink = lot.Jewelry.ImageJewelries.FirstOrDefault().ImageLink
                        };

                        await _unitOfWork.NotificationRepository.AddAsync(notificationloser);
                        await _notificationHub.Clients.Groups(customerLot.Customer.AccountId.ToString()).SendAsync("NewNotificationReceived", "Có thông báo mới!");
                        //cap nhat transaction vi
                        var walletTrasaction = new WalletTransaction
                        {
                            transactionType = EnumTransactionType.RefundDeposit.ToString(),
                            DocNo = customerLot.Id,
                            Amount = customerLot.Lot.Deposit,
                            TransactionTime = DateTime.UtcNow,
                            Status = "Completed",
                            WalletId = customerLot.Customer.Wallet.Id,
                            transactionPerson = customerLot.Customer.Id
                        };
                        await _unitOfWork.WalletTransactionRepository.AddAsync(walletTrasaction);


                        //cap nhat transaction cty
                        var trasaction = new Transaction
                        {
                            TransactionType = EnumTransactionType.RefundDeposit.ToString(),
                            DocNo = customerLot.Id,
                            Amount = customerLot.Lot.Deposit,
                            TransactionTime = DateTime.UtcNow,
                            TransactionPerson = customerLot.CustomerId

                        };
                        await _unitOfWork.TransactionRepository.AddAsync(trasaction);

                    }
                    _unitOfWork.CustomerLotRepository.UpdateRange(listCustomerLot);

                }
                await _unitOfWork.SaveChangeAsync();
            
        }


        public async Task<APIResponseModel> checkPlacebidForReduceBidding(int customerId, int lotId)
        {
            var response = new APIResponseModel();

            try
            {
               var checkBidPrice = await _unitOfWork.BidPriceRepository.GetBidPriceByCustomerAndLot(customerId, lotId);
                if (checkBidPrice)
                {
                    response.Message = $"Have BìdPrice";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = true;
                }              
                else
                {
                    response.Message = $"Don't have BìdPrice";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = false;

                }
            }
            catch (Exception ex)
            {
                response.ErrorMessages = ex.Message.Split(',').ToList();
                response.Message = "Exception";
                response.Code = 500;
                response.IsSuccess = false;
            }
            return response;
        }

        public async Task<APIResponseModel> updateActiveForAutobid(int autobidId, bool isActive)
        {
            var response = new APIResponseModel();

            try
            {
                var autoBid = await _unitOfWork.AutoBidRepository.GetByIdAsync(autobidId);
                if (autoBid != null)
                {
                    autoBid.IsActive = isActive;
                    _unitOfWork.AutoBidRepository.Update(autoBid);
                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        response.Message = $"Update autobid sucessfully";
                        response.Code = 200;
                        response.IsSuccess = true;
                        response.Data = autoBid.IsActive;
                    }
                }
                else
                {
                    response.Message = $"Not found autobid";
                    response.Code = 404;
                    response.IsSuccess = true;                   

                }
            }
            catch (Exception ex)
            {
                response.ErrorMessages = ex.Message.Split(',').ToList();
                response.Message = "Exception";
                response.Code = 500;
                response.IsSuccess = false;
            }
            return response;
        }


    }
}
