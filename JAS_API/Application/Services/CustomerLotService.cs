using Application.Interfaces;
using Application.ServiceReponse;
using Application.ViewModels.CustomerLotDTOs;
using Application.ViewModels.ValuationDTOs;
using AutoMapper;
using CloudinaryDotNet;
using Domain.Entity;
using System;
using System.Collections.Generic;
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
       
        
        public CustomerLotService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
           
        }
        public async Task<APIResponseModel> GetCustomerLotByCustomerAndLot(int customerId, int lotId)
        {
            var response = new APIResponseModel();
            try
            {
                var valuationById = await _unitOfWork.CustomerLotRepository.GetCustomerLotByCustomerAndLot(customerId, lotId);
                if (valuationById != null)
                {
                    var valuation = _mapper.Map<CustomerLotDTO>(valuationById);
                    response.Message = $"Found CustomerLot Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = valuation;
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
    }
}
