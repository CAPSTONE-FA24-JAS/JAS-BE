using Application.Interfaces;
using Application.ViewModels.LotDTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    
    public class LotController : BaseController
    {
        private readonly ILotService _lotService;

        public LotController(ILotService lotService)
        {
            _lotService = lotService;
        }

        [HttpGet]
        public async Task<IActionResult> ViewListLotType()
        {
            var result = await _lotService.GetLotTypes();
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> ViewLotTypeById(int lotTypeId)
        {
            var result = await _lotService.GetLotTypeById(lotTypeId);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> ViewListStatusLot()
        {
            var result = await _lotService.GetListStatusOfLot();
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


        [HttpGet]
        public async Task<IActionResult> ViewListLot()
        {
            var result = await _lotService.GetLots();
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> ViewListLotByAuction(int auctionId)
        {
            var result = await _lotService.GetLotByAuctionId(auctionId);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> ViewDetailLotById(int Id)
        {
            var result = await _lotService.GetLotById(Id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> ViewCustomerLotByLotId(int lotId)
        {
            var result = await _lotService.GetCustomerLotByLot(lotId);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> CheckCustomerInLot(int customerId, int lotId)
        {
            var result = await _lotService.CheckCustomerInLot(customerId,lotId);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLotFixedPrice(CreateLotFixedPriceDTO createLotDTO)
        {
            var result = await _lotService.CreateLot(createLotDTO);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLotSecretAuction(CreateLotSecretAuctionDTO createLotDTO)
        {
            var result = await _lotService.CreateLot(createLotDTO);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLotPublicAuction(CreateLotPublicAuctionDTO createLotDTO)
        {
            var result = await _lotService.CreateLot(createLotDTO);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> CreateLotAuctionPriceGraduallyReduced(CreateLotAuctionPriceGraduallyReducedDTO createLotDTO)
        {
            var result = await _lotService.CreateLot(createLotDTO);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterToBid(RegisterToLotDTO registerToLotDTO)
        {
            var result = await _lotService.RegisterToLot(registerToLotDTO) ;
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CheckCustomerHaveBidPrice([FromBody]RequestCheckCustomerInLotDTO checkCustomerInLotDTO)
        {
            var result = await _lotService.CheckCustomerAuctioned(checkCustomerInLotDTO);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> TotalCustomerInLotFixedPrice(int lotId)
        {
            var result = await _lotService.TotalPlayerInLotFixed(lotId);
            return (!result.IsSuccess)?BadRequest(result):Ok(result);

        }
        
       [HttpGet]
        public async Task<IActionResult> GetPlayerInLotFixedAndSercet(int lotId)
        {
            var result = await _lotService.GetPlayerInLotFixedAndSercet(lotId);
            return (!result.IsSuccess) ? BadRequest(result) : Ok(result);

        }

    }
}
