﻿using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.InvoiceDTOs;
using Application.ViewModels.ValuationDTOs;
using AutoMapper;
using CloudinaryDotNet;
using Domain.Entity;
using Domain.Enums;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private const string Tags = "Backend_ImageDelivery";

        public InvoiceService(IUnitOfWork unitOfWork, IMapper mapper, Cloudinary cloudinary)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinary = cloudinary;
        }

        public async Task<APIResponseModel> getInvoicesByStatusForManger(int status, int? pageSize, int? pageIndex)
        {

            var response = new APIResponseModel();

            try
            {
                                              
                    var statusTranfer = EnumHelper.GetEnums<EnumCustomerLot>().FirstOrDefault(x => x.Value == status).Name;
                    
               

                var invoices = await _unitOfWork.InvoiceRepository.getInvoicesByStatusForManger(statusTranfer, pageSize, pageIndex);
                List<InvoiceDTO> listInvoiceDTO = new List<InvoiceDTO>();
                if (invoices.totalItems > 0)
                {
                    foreach (var item in invoices.data)
                    {
                        var invoicesResponse = _mapper.Map<InvoiceDTO>(item);
                        listInvoiceDTO.Add(invoicesResponse);
                    };


                    var dataresponse = new
                    {
                        DataResponse = listInvoiceDTO,
                        totalItemRepsone = invoices.totalItems
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


        public async Task<APIResponseModel> AsignShipper(int invoiceId, int shipperId, int status)
        {
            var response = new APIResponseModel();
            try
            {
                var invoiceById = await _unitOfWork.InvoiceRepository.GetByIdAsync(invoiceId);
                if (invoiceById != null)
                {
                    invoiceById.CustomerLot.Status = EnumHelper.GetEnums<EnumStatusValuation>().FirstOrDefault(x => x.Value == status).Name;
                    invoiceById.ShipperId = shipperId;

                    invoiceById.Status = EnumHelper.GetEnums<EnumStatusValuation>().FirstOrDefault(x => x.Value == status).Name;
                    _unitOfWork.CustomerLotRepository.Update(invoiceById.CustomerLot);
                    _unitOfWork.InvoiceRepository.Update(invoiceById);

                    await _unitOfWork.SaveChangeAsync();

                    var valuationDTO = _mapper.Map<InvoiceDTO>(invoiceById);

                    response.Message = $"Asign shipper Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = valuationDTO;
                }
                else
                {
                    response.Message = $"Not found invoice";
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

        public async Task<APIResponseModel> GetInvoiceByStatusOfShipper(int shipperId, int status, int? pageSize, int? pageIndex)
        {
            var response = new APIResponseModel();

            try
            {

                Expression<Func<Invoice, bool>> filter;

                if (status != null)
                {
                    var statusTranfer = EnumHelper.GetEnums<EnumStatusValuation>().FirstOrDefault(x => x.Value == status).Name;
                    filter = x => x.ShipperId == shipperId && statusTranfer.Equals(x.Status);
                }
                else
                {
                    filter = x => x.StaffId == shipperId;
                }

                var invoices = await _unitOfWork.InvoiceRepository.GetAllPaging(filter: filter,
                                                                             orderBy: x => x.OrderByDescending(t => t.CreationDate),
                                                                             includeProperties: "CustomerLot",
                                                                             pageIndex: pageIndex,
                                                                             pageSize: pageSize);
                List<InvoiceDTO> listInvoiceDTO = new List<InvoiceDTO>();
                if (invoices.totalItems > 0)
                {
                    foreach (var item in invoices.data)
                    {
                        var invoicesResponse = _mapper.Map<InvoiceDTO>(item);
                        listInvoiceDTO.Add(invoicesResponse);
                    };


                    var dataresponse = new
                    {
                        DataResponse = listInvoiceDTO,
                        totalItemRepsone = invoices.totalItems
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

        public async Task<APIResponseModel> UpdateSuccessfulDeliveryByShipper(SuccessfulDeliveryRequestDTO deliveryDTO)
        {
            var response = new APIResponseModel();
            try
            {
                var invoiceById = await _unitOfWork.InvoiceRepository.GetByIdAsync(deliveryDTO.InvoiceId);
                if (invoiceById != null)
                {
                    invoiceById.CustomerLot.Status = EnumHelper.GetEnums<EnumStatusValuation>().FirstOrDefault(x => x.Value == deliveryDTO.Status).Name;
                    _unitOfWork.CustomerLotRepository.Update(invoiceById.CustomerLot);

                    
                    var uploadImage = await _cloudinary.UploadAsync(new CloudinaryDotNet.Actions.ImageUploadParams
                    {
                        File = new FileDescription(deliveryDTO.ImageDelivery.FileName,
                                                   deliveryDTO.ImageDelivery.OpenReadStream()),
                        Tags = Tags
                    }).ConfigureAwait(false);

                    if (uploadImage == null || uploadImage.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        response.Message = $"Image upload failed." + uploadImage.Error.Message + "";
                        response.Code = (int)uploadImage.StatusCode;
                        response.IsSuccess = false;
                    }
                    else
                    {
                        var statusImvoice = new StatusInvoice
                        {
                            Status = EnumCustomerLot.Delivered.ToString(),
                            CurrentDate = DateTime.Now,
                            InvoiceId = deliveryDTO.InvoiceId,
                            ImageLink = uploadImage.SecureUrl.AbsoluteUri
                        };
                        
                        await _unitOfWork.StatusInvoiceRepository.AddAsync(statusImvoice);
                        await _unitOfWork.SaveChangeAsync();

                        var statusInvoiceDTO = _mapper.Map<StatusInvoiceDTO>(statusImvoice);

                        response.Message = $"Add data in StatusInvoice Successfully";
                        response.Code = 200;
                        response.IsSuccess = true;
                        response.Data = statusInvoiceDTO;
                    }

                    
                }
                else
                {
                    response.Message = $"Not found invoice";
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

        public async Task<APIResponseModel> UpdateStatus(int invoiceId, int status)
        {
            var response = new APIResponseModel();
            try
            {
                var invoiceById = await _unitOfWork.InvoiceRepository.GetByIdAsync(invoiceId);
                if (invoiceById != null)
                {
                    invoiceById.CustomerLot.Status = EnumHelper.GetEnums<EnumStatusValuation>().FirstOrDefault(x => x.Value == status).Name;
                    
                    invoiceById.Status = EnumHelper.GetEnums<EnumStatusValuation>().FirstOrDefault(x => x.Value == status).Name;
                    _unitOfWork.CustomerLotRepository.Update(invoiceById.CustomerLot);
                    _unitOfWork.InvoiceRepository.Update(invoiceById);

                    await _unitOfWork.SaveChangeAsync();

                    var valuationDTO = _mapper.Map<InvoiceDTO>(invoiceById);

                    response.Message = $"Asign shipper Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = valuationDTO;
                }
                else
                {
                    response.Message = $"Not found invoice";
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
