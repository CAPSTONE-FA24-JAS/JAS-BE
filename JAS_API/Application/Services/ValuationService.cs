using Application.Interfaces;
using Application.Repositories;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.ValuationDTOs;
using AutoMapper;
using Azure;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet.Core;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ValuationService : IValuationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private const string Tags = "Backend_ImageValuation";
        private const string Tags_Receipt = "ReceiptPDF";
        public ValuationService(IUnitOfWork unitOfWork, IMapper mapper, Cloudinary cloudinary)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinary = cloudinary;
        }
        public async Task<APIResponseModel> ConsignAnItem(ConsignAnItemDTO consignAnItem)
        {
            var response = new APIResponseModel();
            List<String> imagesValuation = new List<string>();
            try
            {
                var newvaluation = _mapper.Map<Valuation>(consignAnItem);
                if (newvaluation == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Mapper failed";
                }
                else
                {
                    newvaluation.Status = "Requested";
                    await _unitOfWork.ValuationRepository.AddAsync(newvaluation);
                    await _unitOfWork.SaveChangeAsync();

                    foreach (var image in consignAnItem.ImageValuation)
                    {
                        var uploadImage = await _cloudinary.UploadAsync(new CloudinaryDotNet.Actions.ImageUploadParams
                        {
                            File = new FileDescription(image.FileName,
                                                   image.OpenReadStream()),
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
                            var imageValuationinput = new ImageValuationDTO
                            {
                                ValuationId = newvaluation.Id,
                                ImageLink = uploadImage.SecureUrl.AbsoluteUri
                            };
                            imagesValuation.Add(imageValuationinput.ImageLink);
                            var imageValuation = _mapper.Map<ImageValuation>(imageValuationinput);
                            await _unitOfWork.ImageValuationRepository.AddAsync(imageValuation);
                            if (await _unitOfWork.SaveChangeAsync() > 0)
                            {
                                response.Message = $"Image upload Successfull";
                                response.Code = 200;
                                response.IsSuccess = true;
                            }

                        }
                    }

                    response.Message = $"Consign an item Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = imagesValuation;

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

        public async Task<APIResponseModel> GetAllAsync(int? pageSize, int? pageIndex)
        {
            var response = new APIResponseModel();
            try
            {
                var valuations = await _unitOfWork.ValuationRepository.GetAllPaging(filter: null,
                                                                             orderBy: x => x.OrderByDescending(t => t.CreationDate),
                                                                             includeProperties: "Seller,ImageValuations,ValuationDocuments,Staff",
                                                                             pageIndex: pageIndex,
                                                                             pageSize: pageSize);
                List<ValuationDTO> listValuationDTO = new List<ValuationDTO>();
                if (valuations.totalItems > 0 )
                {
                    response.Message = $"List consign items Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    foreach (var item in valuations.data)
                    {
                        var valuationsResponse = _mapper.Map<ValuationDTO>(item);
                        listValuationDTO.Add(valuationsResponse);
                    };                   
                    
                    var dataresponse = new
                    {
                        DataResponse = listValuationDTO,
                        totalItemRepsone = valuations.totalItems
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

        public async Task<APIResponseModel> AssignStaffForValuationAsync(int id, int staffId, string? status)
        {
            var response = new APIResponseModel();
            try
            {
                var valuationById = await _unitOfWork.ValuationRepository.GetByIdAsync(id);
                if(valuationById != null)
                {
                    valuationById.StaffId = staffId;
                    valuationById.Status = status;

                    _unitOfWork.ValuationRepository.Update(valuationById);
                    await _unitOfWork.SaveChangeAsync();

                    response.Message = $"Update status Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = valuationById;
                }
                else
                {
                    response.Message = $"Not found valuation";
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

        public async Task<APIResponseModel> CreatePreliminaryValuationAsync(int id, string status, float preliminaryPrice)
        {
            var response = new APIResponseModel();
            try
            {
                var valuationById = await _unitOfWork.ValuationRepository.GetByIdAsync(id);
                if (valuationById != null)
                {
                    //valuationById.DesiredPrice = preliminaryPrice;
                    valuationById.PricingTime = DateTime.Now;
                    valuationById.Status = status;
                    

                    _unitOfWork.ValuationRepository.Update(valuationById);
                    await _unitOfWork.SaveChangeAsync();

                    response.Message = $"Update status Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = valuationById;
                }
                else
                {
                    response.Message = $"Not found valuation";
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

        public async Task<APIResponseModel> getPreliminaryValuationByIdAsync(int id)
        {
            var response = new APIResponseModel();
            try
            {
                var valuationById = await _unitOfWork.ValuationRepository.GetByIdAsync(id, includes: new Expression<Func<Valuation, 
                                                                                           object>>[] { x => x.ImageValuations, x => x.ValuationDocuments, 
                                                                                                        x => x.Seller, x => x.Staff });
                if (valuationById != null)
                {
                    var valuation = _mapper.Map<ValuationDTO>(valuationById);
                    response.Message = $"Found valuation Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = valuation;
                }
                else
                {
                    response.Message = $"Not found valuation";
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

        public async Task<APIResponseModel> getPreliminaryValuationByStatusOfSellerAsync(int sellerId, string? status, int? pageSize, int? pageIndex)
        {
            var response = new APIResponseModel();
            
            try
            {
                Expression<Func<Valuation, bool>> filter;

                if (status != null)
                {
                    filter = x => x.SellerId == sellerId && status.Equals(x.Status);
                }
                else
                {
                    filter = x => x.SellerId == sellerId;
                }   

                var valuations = await _unitOfWork.ValuationRepository.GetAllPaging(filter: filter,
                                                                             orderBy: x => x.OrderByDescending( t => t.CreationDate),
                                                                             includeProperties: "Seller,ImageValuations,ValuationDocuments,Staff",
                                                                             pageIndex: pageIndex,
                                                                             pageSize: pageSize);
                List<ValuationDTO> listValuationDTO = new List<ValuationDTO>();
                if (valuations.totalItems > 0)
                {
                    foreach (var item in valuations.data)
                    {
                        var valuationsResponse = _mapper.Map<ValuationDTO>(item);
                        listValuationDTO.Add(valuationsResponse);
                    };


                    var dataresponse = new
                    {
                        DataResponse = listValuationDTO,
                        totalItemRepsone = valuations.totalItems
                    };
                    response.Message = $"List consign items Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
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

        public async Task<APIResponseModel> getPreliminaryValuationsByStatusOfStaffAsync(int staffId, string? status, int? pageSize, int? pageIndex)
        {
            var response = new APIResponseModel();
            try
            {
                Expression<Func<Valuation, bool>> filter;

                if (status != null)
                {
                    filter = x => x.StaffId == staffId && status.Equals(x.Status);
                }
                else
                {
                    filter = x => x.StaffId == staffId;
                }

                var valuations = await _unitOfWork.ValuationRepository.GetAllPaging(filter: filter,
                                                                             orderBy: x => x.OrderByDescending(t => t.CreationDate),
                                                                             includeProperties: "Seller,ImageValuations,ValuationDocuments,Staff",
                                                                             pageIndex: pageIndex,
                                                                             pageSize: pageSize);
                List<ValuationDTO> listValuationDTO = new List<ValuationDTO>();
                if (valuations.totalItems > 0)
                {
                    foreach (var item in valuations.data)
                    {
                        
                        var valuationsResponse = _mapper.Map<ValuationDTO>(item);
                        listValuationDTO.Add(valuationsResponse);
                    };


                    var dataresponse = new
                    {
                        DataResponse = listValuationDTO,
                        totalItemRepsone = valuations.totalItems
                    };
                    response.Message = $"List consign items Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
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

        public async Task<APIResponseModel> UpdateStatusForValuationsAsync(int id, string status)
        {
            var response = new APIResponseModel();
            try
            {
                var valuationById = await _unitOfWork.ValuationRepository.GetByIdAsync(id);
                if (valuationById != null)
                {                    
                    valuationById.Status = status;

                    _unitOfWork.ValuationRepository.Update(valuationById);
                    await _unitOfWork.SaveChangeAsync();

                    response.Message = $"Update status Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = valuationById;
                }
                else
                {
                    response.Message = $"Not found valuation";
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

        public async Task<APIResponseModel> CreateRecieptAsync(int id, ReceiptDTO receipt)
        {
            var response = new APIResponseModel();
            try
            {
                var valuationById = await _unitOfWork.ValuationRepository.GetByIdAsync(id, includes: new Expression<Func<Valuation, 
                                                                                                         object>>[] { x => x.ImageValuations, x => x.ValuationDocuments,
                                                                                                                      x => x.Seller, x => x.Staff });
                if (valuationById != null)
                {
                    valuationById.ActualStatusOfJewelry = receipt.ActualStatusOfJewelry;
                    //valuationById.DeliveryDate = receipt.DeliveryDate;

                    _unitOfWork.ValuationRepository.Update(valuationById);
                    await _unitOfWork.SaveChangeAsync();

                    var seller = await _unitOfWork.AccountRepository.GetByIdAsync(valuationById.SellerId);   
                   

                    byte[] pdfBytes = CreatePDFFile.CreatePDF(valuationById, seller);

                    string filePath = $"BienBanXacNhanNhanHang_{valuationById.Id}.pdf";

                    await File.WriteAllBytesAsync(filePath, pdfBytes);

                    var uploadFile = await _cloudinary.UploadAsync(new RawUploadParams
                    {
                        File = new FileDescription(filePath),
                        Tags = Tags_Receipt,
                        Type = "upload"

                    }).ConfigureAwait(false);
                    if (uploadFile == null || uploadFile.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        response.Message = $"File upload failed." + uploadFile.Error.Message + "";
                        response.Code = (int)uploadFile.StatusCode;
                        response.IsSuccess = false;
                    }
                    else
                    {
                        var valuationDoc = new ValuationDocumentDTO
                        {
                            ValuationId = valuationById.Id,
                            ValuationDocumentTypeId = 2,
                            FileDocument = uploadFile.SecureUrl.AbsoluteUri,
                            CreationDate = DateTime.Now,
                            CreatedBy = valuationById.StaffId
                        };
                        var entity = _mapper.Map<ValuationDocument>(valuationDoc);
                        await _unitOfWork.ValuationDocumentRepository.AddAsync(entity);
                        await _unitOfWork.SaveChangeAsync();
                    }
                    var valuation = _mapper.Map<ValuationDTO>(valuationById);

                    response.Message = $"Create Reciept Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = valuation;
                }
                else
                {
                    response.Message = $"Not found valuation";
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
