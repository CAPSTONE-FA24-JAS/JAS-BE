using Application.Interfaces;
using Application.ViewModels.AccountDTOs;
using Application.ViewModels.WatchingDTOs;
using Microsoft.AspNetCore.Mvc;
namespace WebAPI.Controllers
{
    public class WatchingController : BaseController
    {
        private readonly IWatchingService _watchingService;

        public WatchingController(IWatchingService watchingService)
        {
            _watchingService = watchingService;
        }

        [HttpGet]
        public async Task<IActionResult> ViewAllWatchingOfCustomer(int customerId)
        {
            var result = await _watchingService.GetWatchingByCustomer(customerId);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CheckIsWatchingJewelryOfCustomeṛ̣(CreateWatchingDTO createWatchingDTO)
        {
            var result = await _watchingService.checkIsWatchingJewelryOfCustomeṛ̣(createWatchingDTO);
            return (!result.IsSuccess)?BadRequest(result): Ok(result); ;
        }
        
        [HttpPost]
        public async Task<IActionResult> AddNewWatchingForCustomer(CreateWatchingDTO createWatchingDTO)
        {
            var result = await _watchingService.AddNewWatching(createWatchingDTO);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveWatching(int watchingId)
        {
            var result = await _watchingService.RemoveWatching(watchingId);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
