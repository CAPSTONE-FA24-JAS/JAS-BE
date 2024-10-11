using Application.Interfaces;
using Application.Services;
using Application.ViewModels.ArtistDTOs;
using Application.ViewModels.KeyCharacteristicDTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class ArtistsController : BaseController
    {
        private readonly IArtistService _artistService;

        public ArtistsController(IArtistService artistService)
        {
            _artistService = artistService;
        }

        [HttpGet]
        public async Task<IActionResult> ViewArtistsAsync()
        {
            var result = await _artistService.getArtistAsync();
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
        public async Task<IActionResult> CreateArtistAsync(ArtistDTO artistDTO)
        {
            var result = await _artistService.CreateArtistAsync(artistDTO);
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
