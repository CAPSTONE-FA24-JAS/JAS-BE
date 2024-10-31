using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.InvoiceDTOs;
using Application.ViewModels.ValuationDTOs;
using Application.ViewModels.VNPayDTOs;
using Application.ViewModels.WalletDTOs;
using AutoMapper;
using Azure;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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
        private const string Tags_Customer = "Customer_ImageDelivery";
        private const string Tags_Shipper = "Shipper_ImageDelivery";
        private readonly IVNPayService _vNPayService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWalletService _walletService;
        private const string Tags = "Backend_BillTranfer";

        public InvoiceService(IUnitOfWork unitOfWork, IMapper mapper, Cloudinary cloudinary, IVNPayService vNPayService, IHttpContextAccessor httpContextAccessor, IWalletService walletService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinary = cloudinary;
            _vNPayService = vNPayService;
            _httpContextAccessor = httpContextAccessor;
            _walletService = walletService;
        }

        public async Task<APIResponseModel> getInvoicesByStatusForManger(int? pageSize, int? pageIndex)
        {

            var response = new APIResponseModel();

            try
            {
                var invoices = await _unitOfWork.InvoiceRepository.getInvoicesByStatusForManger(pageSize, pageIndex);
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
                    invoiceById.CustomerLot.Status = EnumCustomerLot.Delivering.ToString();
                    invoiceById.ShipperId = shipperId;

                    invoiceById.Status = EnumCustomerLot.Delivering.ToString();
                    _unitOfWork.CustomerLotRepository.Update(invoiceById.CustomerLot);
                    _unitOfWork.InvoiceRepository.Update(invoiceById);

                    var historyCustomerLot = new HistoryStatusCustomerLot
                    {
                        CurrentTime = DateTime.Now,
                        Status = EnumCustomerLot.Delivering.ToString(),
                        CustomerLotId = invoiceById.CustomerLotId
                    };

                    await _unitOfWork.HistoryStatusCustomerLotRepository.AddAsync(historyCustomerLot);
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
                    var statusTranfer = EnumHelper.GetEnums<EnumCustomerLot>().FirstOrDefault(x => x.Value == status).Name;
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
                    invoiceById.CustomerLot.Status = EnumCustomerLot.Delivered.ToString();
                    _unitOfWork.CustomerLotRepository.Update(invoiceById.CustomerLot);

                    invoiceById.Status = EnumCustomerLot.Delivered.ToString();

                    var uploadImage = await _cloudinary.UploadAsync(new CloudinaryDotNet.Actions.ImageUploadParams
                    {
                        File = new FileDescription(deliveryDTO.ImageDelivery.FileName,
                                                   deliveryDTO.ImageDelivery.OpenReadStream()),
                        Tags = Tags_Customer
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
                            Status = "Delivered",
                            CurrentDate = DateTime.Now,
                            InvoiceId = deliveryDTO.InvoiceId,
                            ImageLink = uploadImage.SecureUrl.AbsoluteUri
                        };

                        await _unitOfWork.StatusInvoiceRepository.AddAsync(statusImvoice);

                        //luu status history vao history customerlot
                        var historyCustomerLot = new HistoryStatusCustomerLot
                        {
                            CurrentTime = DateTime.Now,
                            Status = EnumCustomerLot.Delivered.ToString(),
                            CustomerLotId = invoiceById.CustomerLotId
                        };

                        await _unitOfWork.HistoryStatusCustomerLotRepository.AddAsync(historyCustomerLot);

                        await _unitOfWork.SaveChangeAsync();

                        var jewelryOfInvoice = invoiceById.CustomerLot.Lot.Jewelry;

                        var invoiceByDTO = _mapper.Map<InvoiceDetailDTO>(invoiceById, x => x.Items["Jewelry"] = jewelryOfInvoice);


                        response.Message = $"Add data in StatusInvoice Successfully";
                        response.Code = 200;
                        response.IsSuccess = true;
                        response.Data = invoiceByDTO;
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

        public async Task<APIResponseModel> FinishInvoiceByManager(int invoiceId)
        {
            var response = new APIResponseModel();
            try
            {
                var invoiceById = await _unitOfWork.InvoiceRepository.GetByIdAsync(invoiceId);
                if (invoiceById != null)
                {
                    invoiceById.CustomerLot.Status = EnumCustomerLot.Finished.ToString();

                    invoiceById.Status = EnumCustomerLot.Finished.ToString();
                    _unitOfWork.CustomerLotRepository.Update(invoiceById.CustomerLot);
                    _unitOfWork.InvoiceRepository.Update(invoiceById);

                    //hoan coc cho nguoi ban
                    var sellerId = invoiceById.CustomerLot.Lot.Jewelry.Valuation.SellerId;
                    if (sellerId == null)
                    {
                        throw new Exception("Khong tim thay sellerId");
                    }
                    else
                    {
                        //cong vao cho wallet seller
                        var walletOfSeller = await _unitOfWork.WalletRepository.GetByCustomerId(sellerId);

                        walletOfSeller.Balance = walletOfSeller.Balance + (decimal?)invoiceById.CustomerLot.Lot.CurrentPrice;

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


                        //luu history cho history customerlot
                        var historyCustomerLot = new HistoryStatusCustomerLot
                        {
                            CurrentTime = DateTime.Now,
                            Status = EnumCustomerLot.Finished.ToString(),
                            CustomerLotId = invoiceById.CustomerLotId
                        };

                        await _unitOfWork.HistoryStatusCustomerLotRepository.AddAsync(historyCustomerLot);


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
                    invoiceExist.FeeShip = await FindFeeShipByDistanceAsync(model.DistanceToDelivery);
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
        internal async Task<float?> FindFeeShipByDistanceAsync(float distance)
        {
            var feeShips = await _unitOfWork.FeeShipRepository
                .GetAllAsync(x => x.From <= distance && x.To >= distance);

            var feeShip = feeShips.FirstOrDefault();

            return (feeShip == null) ? -1 : feeShip.Free;
        }

        public async Task<APIResponseModel> PaymentInvoiceByWallet(PaymentInvoiceByWalletDTO model)
        {

            var response = new APIResponseModel();

            try
            {
                var invoiceById = await _unitOfWork.InvoiceRepository.GetByIdAsync(model.InvoiceId);
                if (invoiceById != null)
                {
                    var walletExist = await _unitOfWork.WalletRepository.GetByIdAsync(model.WalletId);
                    if (walletExist != null)
                    {
                        var updateResult = await _walletService.UpdateBanlance(model.WalletId, (decimal)model.Amount, false);
                        if (!updateResult.IsSuccess)
                        {
                            response.Message = $"Update balance faild";
                            response.Code = 400;
                            response.IsSuccess = false;
                        }
                        else
                        {
                            var walletTrans = new WalletTransaction()
                            {
                                WalletId = model.WalletId,
                                DocNo = model.InvoiceId,
                                TransactionTime = DateTime.Now,
                                Amount = -model.Amount,
                                Status = EnumStatusTransaction.Completed.ToString(),
                                transactionType = EnumTransactionType.BuyPay.ToString(),
                            };

                            var trans = new Transaction()
                            {
                                Amount = +model.Amount,
                                DocNo = model.InvoiceId,
                                TransactionTime = DateTime.Now,
                                TransactionType = EnumTransactionType.BuyPay.ToString(),
                            };

                            await _unitOfWork.WalletTransactionRepository.AddAsync(walletTrans);
                            await _unitOfWork.TransactionRepository.AddAsync(trans);
                            await _walletService.RefundToWalletForUsersAsync(invoiceById.CustomerLot.Lot);  
                            if (await _unitOfWork.SaveChangeAsync() > 0)
                            {
                                response.Message = $"Update Wallet Successfully";
                                response.Code = 200;
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.Message = $"Update Wallet Fail When Saving";
                                response.Code = 500;
                                response.IsSuccess = false;
                            }
                        }
                    }
                    else
                    {
                        response.Message = $"Don't have wallet";
                        response.Code = 404;
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

        public async Task<APIResponseModel> PaymentInvoiceByBankTransfer(PaymentInvoiceByBankTransferDTO model)
        {
            var response = new APIResponseModel();

            try
            {
                var invoiceById = await _unitOfWork.InvoiceRepository.GetByIdAsync(model.InvoiceId);
                if (invoiceById != null)
                {
                    var walletTrans = new WalletTransaction()
                    {
                        DocNo = model.InvoiceId,
                        TransactionTime = DateTime.Now,
                        Amount = (float)-model.Amount,
                        Status = EnumStatusTransaction.Pending.ToString(),
                        transactionType = EnumTransactionType.Banktransfer.ToString(),
                    };

                   await _unitOfWork.WalletTransactionRepository.AddAsync(walletTrans);
                    
                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        response.Message = $"Add transaction Successfully";
                        response.Code = 200;
                        response.IsSuccess = true;
                    }
                    else
                    {
                        response.Message = $"Add transaction Faild";
                        response.Code = 500;
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
                    // tra về url thanh toán 
                    // => Fe thanh toán xong gọi api refund
                    if (!string.IsNullOrEmpty(paymentUrl))
                    {
                        response.Message = $"SucessFull";
                        response.Code = 200;
                        response.IsSuccess = true;
                        response.Data = paymentUrl;
                    }
                    else
                    {
                        response.Message = $"Payment Fail";
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

        public async Task<APIResponseModel> UpdateImageRecivedJewelryByShipper(int invoiceId, IFormFile imageDelivery)
        {
            var response = new APIResponseModel();
            try
            {
                var invoiceById = await _unitOfWork.InvoiceRepository.GetByIdAsync(invoiceId);
                if (invoiceById != null)
                {
                    invoiceById.CustomerLot.Status = EnumCustomerLot.Delivering.ToString();
                    _unitOfWork.CustomerLotRepository.Update(invoiceById.CustomerLot);


                    var uploadImage = await _cloudinary.UploadAsync(new CloudinaryDotNet.Actions.ImageUploadParams
                    {
                        File = new FileDescription(imageDelivery.FileName,
                                                   imageDelivery.OpenReadStream()),
                        Tags = Tags_Customer
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
                            Status = "Recieved",
                            CurrentDate = DateTime.Now,
                            InvoiceId = invoiceId,
                            ImageLink = uploadImage.SecureUrl.AbsoluteUri
                        };

                        await _unitOfWork.StatusInvoiceRepository.AddAsync(statusImvoice);
                        await _unitOfWork.SaveChangeAsync();

                        var jewelryOfInvoice = invoiceById.CustomerLot.Lot.Jewelry;

                        var invoiceByDTO = _mapper.Map<InvoiceDetailDTO>(invoiceById, x => x.Items["Jewelry"] = jewelryOfInvoice);


                        response.Message = $"Add data in StatusInvoice Successfully";
                        response.Code = 200;
                        response.IsSuccess = true;
                        response.Data = invoiceByDTO;
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

        public async Task<APIResponseModel> GetInvoicesRecivedByShipper(int shipperId, int? pageIndex, int? pageSize)
        {
            var response = new APIResponseModel();

            try
            {

                Expression<Func<Invoice, bool>> filter;



                var invoices = await _unitOfWork.InvoiceRepository.getInvoicesRecivedByShipper(shipperId, pageIndex, pageSize);
                List<InvoiceDetailDTO> listInvoiceDTO = new List<InvoiceDetailDTO>();
                if (invoices.totalItems > 0)
                {
                    foreach (var item in invoices.data)
                    {
                        var jewelryOfInvoice = item.CustomerLot.Lot.Jewelry;
                        var invoicesResponse = _mapper.Map<InvoiceDetailDTO>(item, x => x.Items["Jewelry"] = jewelryOfInvoice);
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

        public async Task<APIResponseModel> UploadBillForPaymentInvoiceByBankTransfer(UploadPaymentInvoiceByBankTransferDTO model)
        {
            var response = new APIResponseModel();
            try
            {
                var invoiceById = await _unitOfWork.InvoiceRepository.GetByIdAsync(model.InvoiceId);
                if (invoiceById != null)
                {
                    var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams
                    {
                        File = new FileDescription(model.FileBill.FileName,
                                                   model.FileBill.OpenReadStream()),
                        Tags = Tags
                    }).ConfigureAwait(false);

                    if (uploadResult == null || uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        response.Message = $"Image upload failed." + uploadResult.Error.Message + "";
                        response.Code = (int)uploadResult.StatusCode;
                        response.IsSuccess = false;
                    }
                    else
                    {
                        invoiceById.LinkBillTransaction = uploadResult.SecureUrl.AbsoluteUri;
                        if (await _unitOfWork.SaveChangeAsync() > 0)
                        {
                            response.Message = $"Upload file bill transaction Successfully";
                            response.Code = 200;
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.Message = $"Upload file bill transaction Faild";
                            response.Code = 500;
                            response.IsSuccess = false;
                        }
                    }
                }
                else
                {
                    response.Message = $"not found invoice";
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

        public async Task<APIResponseModel> ApproveBillForPaymentInvoiceByBankTransfer(int invoiceId)
        {
            var response = new APIResponseModel();
            try
            {
                var invoiceById = await _unitOfWork.InvoiceRepository.GetByIdAsync(invoiceId);
                if (invoiceById != null)
                {
                    var walletTransactions = await _unitOfWork.WalletTransactionRepository.GetAllAsync(x => x.DocNo == invoiceId && x.transactionType == EnumTransactionType.Banktransfer.ToString());
                    var walletTransaction = walletTransactions.FirstOrDefault();
                    if (walletTransaction != null)
                    {
                        walletTransaction.Status = EnumStatusTransaction.Completed.ToString();
                        invoiceById.Status = EnumCustomerLot.Paid.ToString();
                        invoiceById.CustomerLot.Status = EnumCustomerLot.Paid.ToString();

                        var historyStatusCustomerLot = new HistoryStatusCustomerLot()
                        {
                            CustomerLotId = invoiceById.CustomerLot.Id,
                            Status = EnumCustomerLot.Paid.ToString(),
                            CurrentTime = DateTime.UtcNow,
                        };

                        var trans = new Transaction()
                        {
                            Amount = (float)walletTransaction.Amount,
                            DocNo = walletTransaction.DocNo,
                            TransactionTime = DateTime.Now,
                            TransactionType = EnumTransactionType.Banktransfer.ToString(),
                        };

                        await _unitOfWork.TransactionRepository.AddAsync(trans);
                        await _unitOfWork.HistoryStatusCustomerLotRepository.AddAsync(historyStatusCustomerLot);
                        await _walletService.RefundToWalletForUsersAsync(invoiceById.CustomerLot.Lot);
                        if (await _unitOfWork.SaveChangeAsync() > 0)
                        {
                            response.Message = "Upload file bill transaction Successfully";
                            response.Code = 200;
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.Message = "Failed to save changes";
                            response.Code = 500;
                            response.IsSuccess = false;
                        }
                    }
                    else
                    {
                        response.Message = "Wallet transaction records not found for the specified invoice";
                        response.Code = 404;
                        response.IsSuccess = false;
                    }
                }
                else
                {
                    response.Message = "Invoice not found";
                    response.Code = 404;
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessages = ex.Message.Split(',').ToList();
                response.Message = "An exception occurred";
                response.Code = 500;
                response.IsSuccess = false;
            }
            return response;
        }

        public Lot GetLotInInvoice(int invoiceId)
        {
            var lotExit = _unitOfWork.InvoiceRepository.GetByIdAsync(invoiceId).Result.CustomerLot.Lot;
            if (lotExit == null) 
            {
                return null;
            }
            return lotExit;
        }

        public async Task<APIResponseModel> GetListInvoiceForCheckBill()
        {
            var response = new APIResponseModel();

            try
            {

                var invoice = await _unitOfWork.InvoiceRepository.GetAllAsync(condition: x => x.InvoiceOfWalletTransaction.Status == EnumStatusTransaction.Pending.ToString() && x.LinkBillTransaction != null);
                if (invoice.Count > 0)
                {
                    var invoicesResponse = _mapper.Map<IEnumerable<ViewCheckInvoiceHaveBill>>(invoice);
                    response.Message = $"Received Invoice Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = invoicesResponse;
                }
                else
                {
                    response.Message = $"Don't have invoice for check bill";
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
    }
}
