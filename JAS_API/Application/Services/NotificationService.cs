using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.InvoiceDTOs;
using Application.ViewModels.NotificationDTOs;
using Application.ViewModels.ValuationDTOs;
using AutoMapper;
using CloudinaryDotNet;
using Domain.Entity;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private const string Tags = "Backend_ImageProfile";

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper, Cloudinary cloudinary)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinary = cloudinary;
        }
        public async Task<APIResponseModel> getNotificationsByAccountId(int accountId, int? pageIndex = null, int? pageSize = null)
        {
            var response = new APIResponseModel();

            try
            {
                Expression<Func<Notification, bool>> filter;

               
                
                filter = x => x.AccountId == accountId;
                

                var notifications = await _unitOfWork.NotificationRepository.GetAllPaging(filter: filter,
                                                                             orderBy: x => x.OrderByDescending(t => t.CreationDate),
                                                                             includeProperties: "Account",
                                                                             pageIndex: pageIndex,
                                                                             pageSize: pageSize);
               
                List<ViewNotificationDTO> listNotificationDTO = new List<ViewNotificationDTO>();
                if (notifications.totalItems > 0)
                {
                    foreach (var item in notifications.data)
                    {
                        var notificationResponse = _mapper.Map<ViewNotificationDTO>(item);
                        listNotificationDTO.Add(notificationResponse);
                    };


                    var dataresponse = new
                    {
                        DataResponse = listNotificationDTO,
                        totalItemRepsone = notifications.totalItems
                    };
                    response.Message = $"List invoices Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = dataresponse;
                }
                else
                {
                    response.Message = $"Don't have invoices";
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

        public async Task<APIResponseModel> markNotificationAsReadByAccountId(int notificationId)
        {
            var response = new APIResponseModel();
            try
            {
                var notificationById = await _unitOfWork.NotificationRepository.GetByIdAsync(notificationId);
                if (notificationById != null)
                {
                    notificationById.Is_Read = true;
                    var valuation = _mapper.Map<ViewNotificationDTO>(notificationById);
                    response.Message = $"Found notification Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = valuation;
                }
                else
                {
                    response.Message = $"Not found notification";
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

        public async Task<APIResponseModel> getNotificationsByStaffId(int staffId, int? pageIndex = null, int? pageSize = null)
        {
            var response = new APIResponseModel();

            try
            {
                var staff = await _unitOfWork.StaffRepository.GetByIdAsync(staffId);

                Expression<Func<Notification, bool>> filter;



                filter = x => x.AccountId == staff.AccountId;


                var notifications = await _unitOfWork.NotificationRepository.GetAllPaging(filter: filter,
                                                                             orderBy: x => x.OrderByDescending(t => t.CreationDate),
                                                                             includeProperties: "Account",
                                                                             pageIndex: pageIndex,
                                                                             pageSize: pageSize);

                List<ViewNotificationDTO> listNotificationDTO = new List<ViewNotificationDTO>();
                if (notifications.totalItems > 0)
                {
                    foreach (var item in notifications.data)
                    {
                        var notificationResponse = _mapper.Map<ViewNotificationDTO>(item);
                        listNotificationDTO.Add(notificationResponse);
                    };


                    var dataresponse = new
                    {
                        DataResponse = listNotificationDTO,
                        totalItemRepsone = notifications.totalItems
                    };
                    response.Message = $"List invoices Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = dataresponse;
                }
                else
                {
                    response.Message = $"Don't have invoices";
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


        public async Task<APIResponseModel> getNotificationsByAppraiserId(int appraiserId, int? pageIndex = null, int? pageSize = null)
        {
            var response = new APIResponseModel();

            try
            {
                var appraiser = await _unitOfWork.StaffRepository.GetByIdAsync(appraiserId);

                Expression<Func<Notification, bool>> filter;



                filter = x => x.AccountId == appraiser.AccountId;


                var notifications = await _unitOfWork.NotificationRepository.GetAllPaging(filter: filter,
                                                                             orderBy: x => x.OrderByDescending(t => t.CreationDate),
                                                                             includeProperties: "Account",
                                                                             pageIndex: pageIndex,
                                                                             pageSize: pageSize);

                List<ViewNotificationDTO> listNotificationDTO = new List<ViewNotificationDTO>();
                if (notifications.totalItems > 0)
                {
                    foreach (var item in notifications.data)
                    {
                        var notificationResponse = _mapper.Map<ViewNotificationDTO>(item);
                        listNotificationDTO.Add(notificationResponse);
                    };


                    var dataresponse = new
                    {
                        DataResponse = listNotificationDTO,
                        totalItemRepsone = notifications.totalItems
                    };
                    response.Message = $"List invoices Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = dataresponse;
                }
                else
                {
                    response.Message = $"Don't have invoices";
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
