using Application.Interfaces;
using Application.ViewModels.KeyCharacteristicDTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class KeyCharacteristicsController : BaseController
    {
        private readonly IKeyCharacteristicService _keyCharacteristicService;

        public KeyCharacteristicsController(IKeyCharacteristicService keyCharacteristicService)
        {
            _keyCharacteristicService = keyCharacteristicService;
        }

        [HttpGet]
        public async Task<IActionResult> ViewKeyCharacteristicAsync()
        {
            var result = await _keyCharacteristicService.getKeyCharacteristicesAsync();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateKeyCharacteristicAsync(KeyCharacteristicDTO keyCharacteristicDTO)
        {
            var result = await _keyCharacteristicService.CreateKeyCharacteristicAsync(keyCharacteristicDTO);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

    }
}
