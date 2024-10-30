using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.BidPriceDTOs;
using Application.ViewModels.CustomerLotDTOs;
using Application.ViewModels.LiveBiddingDTOs;
using AutoMapper;
using Castle.Core.Resource;
using CloudinaryDotNet;
using Domain.Entity;
using Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Middlewares;

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
                var topBidders = _cacheService.GetSortedSetDataFilter<BidPriceDTO>("BidPrice", l => l.LotId == request.LotId);
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
                    var limitbid = customer.PriceLimit;
                    var lot = _cacheService.GetLotById(conn.LotId);
                    var customerName = customer.LastName +" " + customer.FirstName;
                   
                    if(limitbid.HasValue && limitbid < request.CurrentPrice)
                    {
                        reponse.Message = "giá đặt cao hơn limit bid";
                        reponse.Code = 500;
                        reponse.IsSuccess = false;
                    }
                    else if(limitbid == null)
                    {
                        reponse.Message = "khong tim thay limit bid trong database";
                        reponse.Code = 500;
                        reponse.IsSuccess = false;
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
                        _cacheService.SetSortedSetData<BidPrice>("BidPrice", bidData, request.CurrentPrice);

                        // Truy xuất bảng xếp hạng giảm dần theo giá đấu từ Redis
                        var topBidders = _cacheService.GetSortedSetDataFilter<BidPrice>("BidPrice", l => l.LotId == conn.LotId);
                        var highestBid = topBidders.FirstOrDefault();

                        string lotGroupName = $"lot-{conn.LotId}";
                        //trar về name, giá ĐẤU, thời gian
                        await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPriceForStaff", customerId, customerName, request.CurrentPrice, request.BidTime);

                        await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPrice", customerId, request.CurrentPrice, request.BidTime);

                        await _hubContext.Clients.Group(lotGroupName).SendAsync("SendTopPrice", highestBid.CurrentPrice, highestBid.BidTime);

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
                        reponse.Message = "Place bid successfully!";
                    }                 
                }
                else
                {
                    reponse.IsSuccess = false;
                    reponse.Code = 404;
                    reponse.Message = "Not found ConnectionId!";
                }
            }catch(Exception ex)
            {
                reponse.IsSuccess = false;
                reponse.Code = 500;
                reponse.Message = ex.Message;
            }
            return reponse;
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

                          
                           await _unitOfWork.BidPriceRepository.AddAsync(bidData);

                            //ket thuc lot va cap nhat status, actual time end lot, 
                            var lotSql = await _unitOfWork.LotRepository.GetByIdAsync(conn.LotId);
                            lotSql.Status = EnumStatusLot.Sold.ToString();
                            lotSql.ActualEndTime = request.BidTime;
                            lotSql.CurrentPrice = request.CurrentPrice;
                            _unitOfWork.LotRepository.Update(lotSql);
                        _cacheService.UpdateLotEndTime(lotSql.Id, request.BidTime);
                           
                           
                            
                            //trar về name, giá ĐẤU, thời gian
                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPriceForStaffforReducedBidding", "Phiên đã kết thúc!", customerId, customerName, request.CurrentPrice, request.BidTime);

                            await _hubContext.Clients.Group(lotGroupName).SendAsync("SendBiddingPriceforReducedBidding", "Phiên đã kết thúc!", customerId, request.CurrentPrice, request.BidTime);

                            //Tự động tạo invoice cho winner, hoàn cọc cho người thua, lưu transaction ví, transaction cty,
                            //lưu history cho customerLot, đổi trạng thái invoice và customerLot

                            var winnerCustomerLot = await _unitOfWork.CustomerLotRepository.GetCustomerLotByCustomerAndLot(customerId, conn.LotId);
                            winnerCustomerLot.IsWinner = true;
                            winnerCustomerLot.CurrentPrice = request.CurrentPrice;
                            _unitOfWork.CustomerLotRepository.Update(winnerCustomerLot);
                            //tao invoice cho wwinner
                            var invoice = new Invoice
                            {
                                CustomerId = customerId,
                                CustomerLotId = winnerCustomerLot.Id,
                                StaffId = winnerCustomerLot.Lot.StaffId,
                                Price = request.CurrentPrice,
                                Free = (float?)(request.CurrentPrice * 0.25),
                                TotalPrice = (float?)(request.CurrentPrice + request.CurrentPrice * 0.25 - lotSql.Deposit),
                                CreationDate = DateTime.Now,
                                Status = EnumCustomerLot.CreateInvoice.ToString()
                            };

                            await _unitOfWork.InvoiceRepository.AddAsync(invoice);

                            winnerCustomerLot.Status = EnumCustomerLot.CreateInvoice.ToString();
                            winnerCustomerLot.IsInvoiced = true;
                            _unitOfWork.CustomerLotRepository.Update(winnerCustomerLot);


                            var historyCustomerlot = new HistoryStatusCustomerLot()
                            {
                                Status = winnerCustomerLot.Status,
                                CustomerLotId = winnerCustomerLot.Id,
                                CurrentTime = DateTime.Now,
                            };
                            await _unitOfWork.HistoryStatusCustomerLotRepository.AddAsync(historyCustomerlot);

                            //xu ly cho thang thua
                            var losers = _unitOfWork.CustomerLotRepository.GetListCustomerLotByLotId(conn.LotId, winnerCustomerLot.Id);
                            if (losers != null)
                            {
                                List<CustomerLot> listCustomerLot = new List<CustomerLot>();
                                foreach (var loser in losers)
                                {
                                    loser.IsWinner = false;

                                    //hoan coc cho loser
                                    var walletOfLoser = await _unitOfWork.WalletRepository.GetByCustomerId(loser.CustomerId);
                                    walletOfLoser.Balance = walletOfLoser.Balance + (decimal?)loser.Lot.Deposit;
                                    _unitOfWork.WalletRepository.Update(walletOfLoser);
                                    loser.IsRefunded = true;
                                    loser.Status = EnumCustomerLot.Refunded.ToString();

                                    listCustomerLot.Add(loser);
                                    //lưu history của loser là refunded
                                    var historyCustomerlotLoser = new HistoryStatusCustomerLot()
                                    {
                                        Status = loser.Status,
                                        CustomerLotId = loser.Id,
                                        CurrentTime = DateTime.Now,
                                    };
                                    await _unitOfWork.HistoryStatusCustomerLotRepository.AddAsync(historyCustomerlotLoser);


                                    //cap nhat transaction vi
                                    var walletTrasaction = new WalletTransaction
                                    {
                                        transactionType = EnumTransactionType.RefundDeposit.ToString(),
                                        DocNo = loser.Id,
                                        Amount = lotSql.Deposit,
                                        TransactionTime = DateTime.UtcNow,
                                        Status = "Completed"
                                    };
                                    await _unitOfWork.WalletTransactionRepository.AddAsync(walletTrasaction);


                                    //cap nhat transaction cty
                                    var trasaction = new Transaction
                                    {
                                        TransactionType = EnumTransactionType.RefundDeposit.ToString(),
                                        DocNo = loser.Id,
                                        Amount = lotSql.Deposit,
                                        TransactionTime = DateTime.UtcNow,

                                    };
                                    await _unitOfWork.TransactionRepository.AddAsync(trasaction);
                                    await _unitOfWork.SaveChangeAsync();
                                }
                                _unitOfWork.CustomerLotRepository.UpdateRange(listCustomerLot);
                            }
                            await _unitOfWork.SaveChangeAsync();

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
