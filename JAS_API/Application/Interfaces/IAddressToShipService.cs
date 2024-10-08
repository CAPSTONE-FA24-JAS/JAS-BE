using Application.ServiceReponse;
using Application.ViewModels.AccountDTOs;
using Application.ViewModels.AddressToShipDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAddressToShipService
    {
        public Task<APIResponseModel> CreateAddressToShip(CreateAddressToShipDTO createDTO);
        public Task<APIResponseModel> DeleteAddressToShip(int Id);
        public Task<APIResponseModel> ViewListAddressToShip();
    }
}
