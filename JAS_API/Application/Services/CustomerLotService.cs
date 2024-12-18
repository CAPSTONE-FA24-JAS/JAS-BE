﻿using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.CustomerLotDTOs;
using Application.ViewModels.LotDTOs;
using Application.ViewModels.ValuationDTOs;
using AutoMapper;
using CloudinaryDotNet;
using Domain.Entity;
using Domain.Enums;
using Grpc.Core;
using iTextSharp.text;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{


    public class CustomerLotService : ICustomerLotService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        
        public CustomerLotService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
           
        }

        public async Task<APIResponseModel> GetBidsOfCustomer(int? customerIId, int? status, int? pageIndex, int? pageSize)
        {
            var response = new APIResponseModel();
            try
            {
                var statusTranfer = EnumHelper.GetEnums<EnumCustomerLot>().FirstOrDefault(x => x.Value == status).Name;
                var customerLots = await _unitOfWork.CustomerLotRepository.GetBidsOfCustomer(customerIId, statusTranfer, pageIndex, pageSize);
                
                List<MyBidDTO> listLotDTO = new List<MyBidDTO>();
                if (customerLots.totalItems > 0)
                {
                    response.Message = $"List customerLot Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    foreach (var item in customerLots.data)
                    {

                        
                        var lotsResponse = _mapper.Map<MyBidDTO>(item);
                        var maxBidPriceOfCustomer = await _unitOfWork.BidPriceRepository.GetMaxBidPriceByCustomerIdAndLot(item.CustomerId, item.LotId);
                        if(maxBidPriceOfCustomer == null)
                        {
                            listLotDTO.Add(lotsResponse);
                        }
                        else
                        {
                            lotsResponse.yourMaxBidPrice = maxBidPriceOfCustomer.CurrentPrice;
                            listLotDTO.Add(lotsResponse);
                        }
                        
                        
                    };

                    var dataresponse = new
                    {
                        DataResponse = listLotDTO,
                        totalItemRepsone = customerLots.totalItems
                    };

                    response.Data = dataresponse;
                }
                else
                {
                    response.Message = $"Don't have customerLot";
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

        public async Task<APIResponseModel> GetCustomerLotByCustomerAndLot(int customerId, int lotId)
        {
            var response = new APIResponseModel();
            try
            {
                var customerLotById = await _unitOfWork.CustomerLotRepository.GetCustomerLotByCustomerAndLot(customerId, lotId);
                if (customerLotById != null)
                {
                    var customerLot = _mapper.Map<CustomerLotByIdDTO>(customerLotById);
                    response.Message = $"Found CustomerLot Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = customerLot;
                }
                else
                {
                    response.Message = $"Not found CustomerLot";
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

        public async Task<APIResponseModel> GetMyBidByCustomerLotId(int customerLotId)
        {
            var response = new APIResponseModel();
            try
            {
                var customerLotById = await _unitOfWork.CustomerLotRepository.GetByIdAsync(customerLotId);
                if (customerLotById != null)
                {
                    var customerLot = _mapper.Map<MyBidDetailDTO>(customerLotById);
                    response.Message = $"Found CustomerLot Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = customerLot;
                }
                else
                {
                    response.Message = $"Not found CustomerLot";
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

        public async Task<APIResponseModel> GetPastBidOfCustomer(int customerIId, List<int> status, int? pageIndex, int? pageSize)
        {
            var response = new APIResponseModel();
            try
            {
                var statusTranfer = EnumHelper.GetEnums<EnumCustomerLot>()
                                              .Where(x => status.Contains(x.Value))
                                              .Select(x => x.Name);
               
                var customerLots = await _unitOfWork.CustomerLotRepository.GetPastBidOfCustomer(customerIId, statusTranfer, pageIndex, pageSize);

                List<MyBidDTO> listLotDTO = new List<MyBidDTO>();
                if (customerLots.totalItems > 0)
                {
                    response.Message = $"List consign items Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    foreach (var item in customerLots.data)
                    {
                        var lotsResponse = _mapper.Map<MyBidDTO>(item);
                        var maxBidPriceOfCustomer = await _unitOfWork.BidPriceRepository.GetMaxBidPriceByCustomerIdAndLot(item.CustomerId, item.LotId);
                        if (maxBidPriceOfCustomer == null)
                        {
                            listLotDTO.Add(lotsResponse);
                        }
                        else
                        {
                            lotsResponse.yourMaxBidPrice = maxBidPriceOfCustomer.CurrentPrice;
                            listLotDTO.Add(lotsResponse);
                        }
                    };

                    var dataresponse = new
                    {
                        DataResponse = listLotDTO,
                        totalItemRepsone = customerLots.totalItems
                    };

                    response.Data = dataresponse;
                }
                else
                {
                    response.Message = $"Don't have valuations";
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
        
        public async Task<(bool, float?)> CheckBidPriceTop(float priceFuture,float highestBidOfLot, AutoBid autoBid)
        {
            try
            {
                //string redisKey = $"BidPrice:{autoBid.CustomerLot.LotId}";
                ////lay ra highest bidPrice
                //var topBidders = _cacheService.GetSortedSetDataFilter<BidPrice>(redisKey, l => l.LotId == autoBid.CustomerLot.LotId);
                //var highestBid = topBidders.FirstOrDefault();
                var autobidCurrent = autoBid;
                if (priceFuture >= autobidCurrent.MinPrice && priceFuture <= autobidCurrent.MaxPrice)
                {
                    if (highestBidOfLot < priceFuture)
                    {
                        return (true, priceFuture); // lấy giá future
                    }
                    else
                    {
                        float? priceCurrentPlusStep = highestBidOfLot; // Giá hiện tại
                        return (false, priceCurrentPlusStep);
                    }
                }
                return (false, null);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
        public async Task<APIResponseModel> UpdateAutoBidPrice(int customerLotId, float priceCurrent)
        {
            var response = new APIResponseModel();
            try
            {
                //Redis
                var winnerCurrentNew = await _unitOfWork.CustomerLotRepository.GetByIdAsync(customerLotId);
               

                BiddingInputDTO bidData = new BiddingInputDTO
                {
                    CurrentPrice = priceCurrent,
                    BidTime = DateTime.UtcNow
                };
                //Redis
                string redisKey = $"BidPrice:{winnerCurrentNew.Lot.Id}";
                _cacheService.AddToStream((int)winnerCurrentNew.LotId, bidData, (int)winnerCurrentNew.CustomerId);
                foreach (var player in await _unitOfWork.CustomerLotRepository.GetAllAsync(x => x.CustomerId != winnerCurrentNew.CustomerId))
                {
                    player.IsWinner = false;
                }

                //winnerCurrentNew.IsWinner = true;
                //winnerCurrentNew.CurrentPrice = priceCurrent;
                //winnerCurrentNew.ModificationDate = DateTime.UtcNow;
                //winnerCurrentNew.Lot.CurrentPrice = priceCurrent;

                if (await _unitOfWork.SaveChangeAsync() > 0)
                {
                    response.Message = $"Update New Winner By AutoBid Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                }
                else
                {
                    response.Message = $"Update New Winner By AutoBid Faild";
                    response.Code = 400;
                    response.IsSuccess = false;
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
        
        public async Task<bool> CheckTimeAutoBid(int customerLotId)
        {
            try
            {
                //Redis
                var playerCurent = await _unitOfWork.CustomerLotRepository
                    .GetByIdAsync(customerLotId);
                var timeOld = playerCurent.ModificationDate.HasValue ? playerCurent.ModificationDate.Value : playerCurent.CreationDate;
                int timeCount = (int)(DateTime.UtcNow - timeOld).TotalSeconds;
                var autoBid = playerCurent.AutoBids.FirstOrDefault(x => x.CustomerLotId == customerLotId && x.IsActive == true);
                int timeIncrement = (int)autoBid.TimeIncrement;

                if (timeCount >= timeIncrement)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async void CreateHistoryCustomerLot(HistoryStatusCustomerLot model)
        {
            try
            {
                await _unitOfWork.HistoryStatusCustomerLotRepository.AddAsync(model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<APIResponseModel> GetWinnerForLot(int lotid)
        {
            var response = new APIResponseModel();
            try
            {


                var customerLots = await _unitOfWork.CustomerLotRepository.GetAllAsync(x => x.IsWinner == true && x.LotId == lotid);
                if(customerLots != null)
                {
                    List<CustomerLotWinnerDTO> listLotDTO = new List<CustomerLotWinnerDTO>();

                    foreach (var item in customerLots)
                    {
                        var lotsResponse = _mapper.Map<CustomerLotWinnerDTO>(item);
                        listLotDTO.Add(lotsResponse);
                    };

                    response.Message = $"Have winner for lot";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = listLotDTO;

                }
                else
                {
                    response.Message = $"Don't have customerLots";
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
