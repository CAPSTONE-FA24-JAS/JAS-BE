using Application.Interfaces;
using Application.ServiceReponse;
using Application.ViewModels.DistrictDTOs;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DistrictService : IDistrictService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DistrictService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<APIResponseModel> CreateNewDistrict(CreateDistrictDTO createDistrictDTO)
        {
            throw new NotImplementedException();
        }

        public Task<APIResponseModel> ViewListDistrict()
        {
            throw new NotImplementedException();
        }
    }
}
