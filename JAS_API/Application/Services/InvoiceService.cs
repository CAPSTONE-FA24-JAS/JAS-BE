using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.ValuationDTOs;
using AutoMapper;
using CloudinaryDotNet;
using Domain.Entity;
using Domain.Enums;
using iTextSharp.text;
using System;
using System.Collections.Generic;
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
                Expression<Func<Invoice, bool>> filter;

                
                
                    var statusTranfer = EnumHelper.GetEnums<EnumStatusValuation>().FirstOrDefault(x => x.Value == status).Name;
                    filter = x => statusTranfer.Equals(x.Status);
               

                var invoices = await _unitOfWork.InvoiceRepository.GetAllPaging(filter: filter,
                                                                             orderBy: x => x.OrderByDescending(t => t.CreationDate),
                                                                             includeProperties: "AddressToShip",
                                                                             pageIndex: pageIndex,
                                                                             pageSize: pageSize);
                List<ValuationDTO> listValuationDTO = new List<ValuationDTO>();
                if (invoices.totalItems > 0)
                {
                    foreach (var item in invoices.data)
                    {
                        var valuationsResponse = _mapper.Map<ValuationDTO>(item);
                        listValuationDTO.Add(valuationsResponse);
                    };


                    var dataresponse = new
                    {
                        DataResponse = listValuationDTO,
                        totalItemRepsone = invoices.totalItems
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
    }
}
