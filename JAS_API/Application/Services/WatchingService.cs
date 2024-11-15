using Application.Interfaces;
using Application.ServiceReponse;
using Application.ViewModels.CategoryDTOs;
using Application.ViewModels.WatchingDTOs;
using AutoMapper;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class WatchingService : IWatchingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WatchingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<APIResponseModel> AddNewWatching(CreateWatchingDTO model)
        {
            var response = new APIResponseModel();
            try
            {
                if( _unitOfWork.WatchingRepository.GetAllAsync(x => x.CustomerId == model.CustomerId && x.JewelryId == model.JewelryId).Result.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "You Are Watched This Jewelry.";
                    return response;
                }
                var watching = _mapper.Map<Watching>(model);
                if (watching == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Mapper failed";
                }
                else
                {
                    await _unitOfWork.WatchingRepository.AddAsync(watching);
                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        response.Message = $"Create Watching Successfully";
                        response.Code = 200;
                        response.IsSuccess = true;
                    }
                    else
                    {
                        response.Message = $"Save DB failed!";
                        response.Code = 400;
                        response.IsSuccess = false;
                    }
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

        public async Task<APIResponseModel> checkIsWatchingJewelryOfCustomeṛ̣(CreateWatchingDTO model)
        {
            var response = new APIResponseModel();
            try
            {

                var ExitsWatchings = await _unitOfWork.WatchingRepository.GetAllAsync(x => x.CustomerId == model.CustomerId && x.JewelryId == model.JewelryId);
                if (ExitsWatchings.Any())
                {
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Message = "Customer is watched this jewelry";
                    response.Data = _mapper.Map<ViewWatchingDTO>(ExitsWatchings.FirstOrDefault());
                }
                else
                {
                    response.Message = $"Customer isn't watching this jewelry";
                    response.Code = 400;
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

        public async Task<APIResponseModel> GetWatchingByCustomer(int customerId)
        {
            var response = new APIResponseModel();
            try
            {

                var watchings = await _unitOfWork.WatchingRepository.GetAllAsync(x => x.CustomerId == customerId);
                if (!watchings.Any())
                {
                    response.Code = 404;
                    response.IsSuccess = true;
                    response.Message = "Not Found List Is Empty";
                }
                else
                {
                    response.Data = _mapper.Map<IEnumerable<ViewWatchingDTO>>(watchings);
                    response.Message = $"Received List Watching Successfully";
                    response.Code = 200;
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

        public async Task<APIResponseModel> RemoveWatching(int watchingId)
        {
            var response = new APIResponseModel();
            try
            {

                var watching = await _unitOfWork.WatchingRepository.GetByIdAsync(watchingId);
                if (watching == null)
                {
                    response.Code = 404;
                    response.IsSuccess = false;
                    response.Message = "Not Found Watching For Remove.";
                }
                else
                {
                    _unitOfWork.WatchingRepository.Remove(watching);
                    if(await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        response.Code = 200;
                        response.IsSuccess = true;
                        response.Message = "Remove Watching Successfuly.";
                    }
                    else
                    {
                        response.Code = 400;
                        response.IsSuccess = false;
                        response.Message = "Remove Watching Faild.";
                    }
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
