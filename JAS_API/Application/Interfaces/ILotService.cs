using Application.ServiceReponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ILotService
    {
        Task<APIResponseModel> CreateLot(object lotDTO);
        Task<APIResponseModel> GetLotTypes();
        Task<APIResponseModel> GetLotTypeById(int lotTypeId);

    }
}
