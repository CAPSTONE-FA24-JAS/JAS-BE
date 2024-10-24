using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.InvoiceDTOs;
using Application.ViewModels.ValuationDTOs;
using Application.ViewModels.VNPayDTOs;
using Application.ViewModels.WalletDTOs;
using AutoMapper;
using CloudinaryDotNet;
using Domain.Entity;
using Domain.Enums;
using iTextSharp.text;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Linq.Expressions;
using System.Net.WebSockets;
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
        private readonly IVNPayService _vNPayService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InvoiceService(IUnitOfWork unitOfWork, IMapper mapper, Cloudinary cloudinary, IVNPayService vNPayService, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinary = cloudinary;
            _vNPayService = vNPayService;
            _httpContextAccessor = httpContextAccessor;

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

        public async Task<APIResponseModel> FinishInvoiceByManager(int invoiceId, int status)
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

                    //hoan coc cho nguoi ban
                    var sellerId = invoiceById.CustomerLot.Lot.Jewelry.Valuation.SellerId;
                    if(sellerId == null)
                    {
                        throw new Exception("Khong tim thay sellerId");
                    }
                    else
                    {
                        //cong vao cho wallet seller
                        var walletOfSeller = await _unitOfWork.WalletRepository.GetByCustomerId(sellerId);

                        walletOfSeller.Balance = walletOfSeller.Balance + (decimal?)invoiceById.CustomerLot.Lot.Deposit;

                        _unitOfWork.WalletRepository.Update(walletOfSeller);

                        //lưu transation vi seller
                        var wallerTransaction = new WalletTransaction
                        {
                            transactionType = EnumTransactionType.SellerPay.ToString(),
                            DocNo = invoiceById.Id,
                            Amount = invoiceById.CustomerLot.Lot.Deposit,
                            TransactionTime = DateTime.Now,
                            Status = "+"
                        };

                        await _unitOfWork.WalletTransactionRepository.AddAsync(wallerTransaction);


                        //luu transaction cho cong ty
                        var transactionCompany = new Transaction
                        {
                            DocNo = invoiceById.Id,
                            Amount = invoiceById.CustomerLot.Lot.Deposit,
                            TransactionTime = wallerTransaction.TransactionTime,
                            TransactionType = EnumTransactionType.SellerPay.ToString(),
                        };
                        await _unitOfWork.TransactionRepository.AddAsync(transactionCompany);


                        await _unitOfWork.SaveChangeAsync();

                        var valuationDTO = _mapper.Map<InvoiceDTO>(invoiceById);

                        response.Message = $"Finish invoice!";
                        response.Code = 200;
                        response.IsSuccess = true;
                        response.Data = valuationDTO;
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

        public async Task<APIResponseModel> getInvoicesByStatusForCustomer(int customerId, int? status, int? pageSize, int? pageIndex)
        {
            var response = new APIResponseModel();

            try
            {
                string statusTranfer = null;
                if (status == null)
                {
                    statusTranfer = string.Empty;
                }
                else
                {
                    statusTranfer = EnumHelper.GetEnums<EnumCustomerLot>().FirstOrDefault(x => x.Value == status).Name;
                }
               
                var invoices = await _unitOfWork.InvoiceRepository.getInvoicesByStatusForCustomer(customerId, statusTranfer, pageSize, pageIndex);
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

        public async Task<APIResponseModel> GetInvoiceDetail(int Id)
        {
            var response = new APIResponseModel();

            try
            {

                var invoiceExist = await _unitOfWork.InvoiceRepository.GetByIdAsync(Id);
                var jewelryOfInvoice = invoiceExist.CustomerLot.Lot.Jewelry;
                if (invoiceExist != null)
                {
                    var invoicesResponse = _mapper.Map<InvoiceDetailDTO>(invoiceExist, x => x.Items["Jewelry"] = jewelryOfInvoice);
                    response.Message = $"Received Invoice Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = invoicesResponse;
                }
                else
                {
                    response.Message = $"Don't have invoice";
                    response.Code = 404;
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

        public async Task<APIResponseModel> UpdateAddressToshipForInvoice(UpdateAddressToShipInvoice model)
        {
            var response = new APIResponseModel();

            try
            {

                var invoiceExist = await _unitOfWork.InvoiceRepository.GetByIdAsync(model.InvoiceId);
                if (invoiceExist != null)
                {
                    invoiceExist.AddressToShipId = model.AddressToShipId;
                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        response.Message = $"Update Invoice Successfully";
                        response.Code = 200;
                        response.IsSuccess = true;
                    }
                    else
                    {
                        response.Message = $"Update Invoice Fail When Saving";
                        response.Code = 500;
                        response.IsSuccess = false;
                    }
                }
                else
                {
                    response.Message = $"Don't have invoice";
                    response.Code = 404;
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

        public Task<APIResponseModel> PaymentInvoiceByWallet(PaymentInvoiceByWalletDTO model)
        {
            throw new NotImplementedException();
        }

        public Task<APIResponseModel> PaymentInvoiceByBankTransfer(PaymentInvoiceByBankTransferDTO model)
        {
            throw new NotImplementedException();
        }

        public async Task<APIResponseModel> PaymentInvoiceByVnPay(PaymentInvoiceByVnPayDTO model)
        {
            var response = new APIResponseModel();

            try
            {

                var invoiceExist = await _unitOfWork.InvoiceRepository.GetByIdAsync(model.InvoiceId);
                if (invoiceExist != null)
                {
                    var vnPayModel = new VNPaymentRequestDTO
                    {
                        Amount = (float)model.Amount,
                        CreatedDate = DateTime.UtcNow,
                        Description = $"payment the invoice have id is : {model.InvoiceId}",
                        FullName = invoiceExist.Customer.FirstName + " " + invoiceExist.Customer.LastName,
                        OrderId = new Random().Next(1000, 100000)
                    };
                    var transaction = new WalletTransaction()
                    {
                        transactionType = EnumTransactionType.BuyPay.ToString(),
                        DocNo = model.InvoiceId,
                    };
                    var httpContext = _httpContextAccessor.HttpContext;
                    string paymentUrl = await _vNPayService.CreatePaymentUrl(httpContext, vnPayModel, transaction);

                    if (!string.IsNullOrEmpty(paymentUrl))
                    {
                        response.Message = $"SucessFull";
                        response.Code = 200;
                        response.IsSuccess = true;
                        response.Data = paymentUrl;
                    }
                    else
                    {
                        response.Message = $"Invoice Fail";
                        response.Code = 500;
                        response.IsSuccess = false;
                    }
                }
                else
                {
                    response.Message = $"Don't have invoice";
                    response.Code = 404;
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
    }
}
