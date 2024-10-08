using Application.Interfaces;
using Application.ServiceReponse;
using Application.ViewModels.ProvinceDTOs;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ProvinceService : IProvinceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProvinceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<APIResponseModel> CreateNewProvince(CreateProvinceDTO createProvinceDTO)
        {
            throw new NotImplementedException();
        }

        public Task<APIResponseModel> ViewListProvince()
        {
            throw new NotImplementedException();
        }
    }
}
