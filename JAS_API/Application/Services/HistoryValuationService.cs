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
    public class HistoryValuationService : IHistoryValuationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HistoryValuationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;           
        }
        public async Task<APIResponseModel> getDetailHistoryValuation(int valuationId)
        {
            var response = new APIResponseModel();

            try
            {
                Expression<Func<HistoryValuation, bool>> filter;

                
                    filter = x => x.ValuationId == valuationId;
                
                

                var historyvaluations = await _unitOfWork.HistoryValuationRepository.GetAllPaging(filter: filter,
                                                                             orderBy: x => x.OrderByDescending(t => t.CreationDate));
                List<HistoryValuationDTO> listHistoryDTO = new List<HistoryValuationDTO>();
                if (historyvaluations.totalItems > 0)
                {
                    foreach (var item in historyvaluations.data)
                    {
                        var historyResponse = _mapper.Map<HistoryValuationDTO>(item);
                        listHistoryDTO.Add(historyResponse);
                    };
                    response.Message = $"List consign items Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = listHistoryDTO;
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
