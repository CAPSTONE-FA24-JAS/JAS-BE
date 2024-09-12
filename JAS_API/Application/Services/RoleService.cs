using Application.Interfaces;
using Application.ServiceReponse;
using Application.ViewModels.AccountDTO;
using Application.ViewModels.RoleDTO;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<APIResponseModel> ViewListRole()
        {
            var reponse = new APIResponseModel();
            try
            {
                var roles = await _unitOfWork.RoleRepository.GetAllAsync();
                if(roles.Count > 0 || roles != null)
                {
                    reponse.IsSuccess = true;
                    reponse.Message = "Received list role successfull";
                    reponse.Code = 200;
                    reponse.Data = _mapper.Map<IEnumerable<RoleDTO>>(roles);
                }
                else
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Received list role faild";
                    reponse.Code = 400;
                }
            }
            catch (Exception ex)
            {
                reponse.ErrorMessages = new List<string> { ex.Message };
                reponse.Message = "Exception";
                reponse.IsSuccess = false;
            }
            return reponse;
        }
    }
}
