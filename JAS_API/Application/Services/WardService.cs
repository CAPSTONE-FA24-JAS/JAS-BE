using Application.Interfaces;
using Application.ServiceReponse;
using Application.ViewModels.WardDTOs;
using AutoMapper;

namespace Application.Services
{
    public class WardService : IWardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WardService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<APIResponseModel> CreateNewWard(CreateWardDTO createWardDTO)
        {
            throw new NotImplementedException();
        }

        public Task<APIResponseModel> ViewListWard()
        {
            throw new NotImplementedException();
        }
    }
}
