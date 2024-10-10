using Application.ServiceReponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IHistoryValuationService
    {
        public Task<APIResponseModel> getDetailHistoryValuation(int id);
    }
}
