using Application.ServiceReponse;
using Application.ViewModels.BidLimitDTOs;
using Application.ViewModels.ProvinceDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IProvinceService
    {
        public Task<APIResponseModel> CreateNewProvince(CreateProvinceDTO createProvinceDTO);
        public Task<APIResponseModel> ViewListProvince();
    }
}
