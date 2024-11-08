using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class NotificationsController : BaseController
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpGet]
        public async Task<IActionResult> getNotificationsByAccountAsync(int accountId, int? pageIndex = null, int? pageSize = null)
        {
            var result = await _notificationService.getNotificationsByAccountId(accountId, pageIndex, pageSize);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> markNotificationAsReadByAccountAsync(int notificationId)
        {
            var result = await _notificationService.markNotificationAsReadByAccountId(notificationId);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpGet]
        public async Task<IActionResult> getNotificationsByStaffAsync(int staffId, int? pageIndex = null, int? pageSize = null)
        {
            var result = await _notificationService.getNotificationsByStaffId(staffId, pageIndex, pageSize);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> getNotificationsByAppraiserAsync(int appraiserId, int? pageIndex = null, int? pageSize = null)
        {
            var result = await _notificationService.getNotificationsByAppraiserId(appraiserId, pageIndex, pageSize);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

    }
}
