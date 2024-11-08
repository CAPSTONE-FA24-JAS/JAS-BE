using Application.ServiceReponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface INotificationService
    {
        Task<APIResponseModel> getNotificationsByAccountId(int accountId, int? pageIndex = null, int? pageSize = null);
        Task<APIResponseModel> markNotificationAsReadByAccountId(int notificationId);
        Task<APIResponseModel> getNotificationsByAppraiserId(int appraiserId, int? pageIndex = null, int? pageSize = null);

        Task<APIResponseModel> getNotificationsByStaffId(int staffId, int? pageIndex = null, int? pageSize = null);
    }
}
