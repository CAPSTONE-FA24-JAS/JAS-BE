﻿using Application.ServiceReponse;
using Application.ViewModels.InvoiceDTOs;
using Application.ViewModels.VNPayDTOs;
using Application.ViewModels.WalletDTOs;
using Domain.Entity;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IInvoiceService
    {
        public Task<APIResponseModel> getInvoicesByStatusForManger(int?pageSize, int? pageIndex);
        public Task<APIResponseModel> AsignShipper(int invoiceId, int shipperId, int status);
        public Task<APIResponseModel> GetInvoiceByStatusOfShipper(int shipperId, int status, int? pageSize, int? pageIndex);
        public Task<APIResponseModel> UpdateImageRecivedJewelryByShipper(int invoiceId, IFormFile imageDelivery);
        public Task<APIResponseModel> UpdateSuccessfulDeliveryByShipper(SuccessfulDeliveryRequestDTO deliveryDTO);
        public Task<APIResponseModel> FinishInvoiceByManager(int invoiceId);
        public Task<APIResponseModel> getInvoicesByStatusForCustomer(int customerId, int? status, int? pageSize, int? pageIndex);
        Task<APIResponseModel> GetInvoiceDetail(int Id);
        Task<APIResponseModel> UpdateAddressToshipForInvoice(UpdateAddressToShipInvoice model);
        Task<APIResponseModel> PaymentInvoiceByWallet(PaymentInvoiceByWalletDTO model);
        Task<APIResponseModel> PaymentInvoiceByBankTransfer(PaymentInvoiceByBankTransferDTO model);
        Task<APIResponseModel> PaymentInvoiceByVnPay(PaymentInvoiceByVnPayDTO model);
        Task<APIResponseModel> UploadBillForPaymentInvoiceByBankTransfer(UploadPaymentInvoiceByBankTransferDTO model);
        Task<APIResponseModel> ApproveBillForPaymentInvoiceByBankTransfer(int invoiceId);
        Task<APIResponseModel>  GetInvoicesRecivedByShipper(int shipperId, int? pageIndex, int? pageSize);
        Task<APIResponseModel> GetListInvoiceForCheckBill();
        Lot GetLotInInvoice(int invoiceId);
        
        Task<APIResponseModel> getInvoicesDeliveringByShipper(int shipperId, int? pageIndex, int? pageSize);
        
        Task<Invoice?> GetInvoiceById(int id);
        Task<APIResponseModel> GetShipperAndInvoices();
        Task<APIResponseModel> getInvoicesDeliveringByShipperToAssign(int? pageIndex, int? pageSize);

        public Task<APIResponseModel> CancelledInvoiceByManager(int invoiceId, string reason);

        public Task<APIResponseModel> UpdateRejectedInvoiceByShipper(int invoiceId, string reason);

        public Task<APIResponseModel> ClosedInvoiceByManager(int invoiceId);
        Task<APIResponseModel> CancelledInvoiceByBuyer(int invoiceId, string reason);

        

    }
}
