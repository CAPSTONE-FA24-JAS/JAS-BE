using Application.Interfaces;
using Application.Services;
using Application.ViewModels.AddressToShipDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class AddressToShipController : BaseController
    {
        private readonly IAddressToShipService _addressToShipService;

        public AddressToShipController(IAddressToShipService addressToShipService)
        {
            _addressToShipService = addressToShipService;
        }

        [HttpGet]
        public async Task<IActionResult> ViewListAddressToShip()
        {
            var result = await _addressToShipService.ViewListAddressToShip();
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
        public async Task<IActionResult> CreateAddressToShip(CreateAddressToShipDTO createAddressToShipDTO)
        {
            var result = await _addressToShipService.CreateAddressToShip(createAddressToShipDTO);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpPut]
        public async Task<IActionResult> DeleteAddressToShip(int Id)
        {
            var result = await _addressToShipService.DeleteAddressToShip(Id);
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
