using Application.Interfaces;
using Application.ViewModels.ValuationDTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class ValuationsController : BaseController
    {
        private readonly IValuationService _valuationService;
        private readonly IHistoryValuationService _historyValuationService;

        public ValuationsController(IValuationService valuationService, IHistoryValuationService historyValuationService)
        {
            _valuationService = valuationService;
            _historyValuationService = historyValuationService;
        }

        [HttpPost]
        public async Task<IActionResult> consignAnItem(ConsignAnItemDTO consignAnItem)
        {
            var result = await _valuationService.ConsignAnItem(consignAnItem);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> getValuationsAsync(int? pageSize, int? pageIndex)
        {
            var result = await _valuationService.GetAllAsync(pageSize, pageIndex);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }


        //chi dinh staff cho consignItem
        [HttpPut]
        public async Task<IActionResult> AssignStaffForValuationAsync(int id, int staffId, int status)
        {
            var result = await _valuationService.AssignStaffForValuationAsync(id, staffId, status);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }


        [HttpPut]
        public async Task<IActionResult> RequestPreliminaryValuationAsync(int id, int status)
        {
            var result = await _valuationService.RequestPreliminaryValuationAsync(id, status);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetRequestPreliminaryValuationAsync(int? pageSize, int? pageIndex)
        {
            var result = await _valuationService.GetRequestPreliminaryValuationAsync(pageSize, pageIndex);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }


        //tao dinh gia so 
        [HttpPut]
        public async Task<IActionResult> createPreliminaryPriceAsync(int id, int status, float EstimatePriceMin, float EstimatePriceMax, int appraiserId)
        {
            var result = await _valuationService.CreatePreliminaryValuationAsync(id, status, EstimatePriceMin, EstimatePriceMax, appraiserId);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }


        //xem Detail valuation 
        [HttpGet]
        public async Task<IActionResult> getValuationByIdAsync(int valuationId)
        {
            var result = await _valuationService.getPreliminaryValuationByIdAsync(valuationId);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }


        //staff xem all consign item, all dinh gia so bo.Neu status null thi hien thi het consign item
        [HttpGet]
        public async Task<IActionResult> getPreliminaryValuationsByStatusOfStaffAsync(int staffId, int? status, int? pageSize, int? pageIndex)
        {
            var result = await _valuationService.getPreliminaryValuationsByStatusOfStaffAsync(staffId, status, pageSize, pageIndex);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> getPreliminaryValuationsOfStaffAsync(int staffId, int? pageSize, int? pageIndex)
        {
            var result = await _valuationService.getPreliminaryValuationsOfStaffAsync(staffId, pageSize, pageIndex);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }


        [HttpGet]
        public async Task<IActionResult> getPreliminaryValuationByStatusOfAppraiserAsync(int appraiserId, int? status, int? pageSize, int? pageIndex)
        {
            var result = await _valuationService.getPreliminaryValuationByStatusOfAppraiserAsync(appraiserId, status, pageSize, pageIndex);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }



        //seller xem all consign item, all dinh gia so bo.Neu status null thi hien thi het consign item

        [HttpGet]
        public async Task<IActionResult> getPreliminaryValuationByStatusOfSellerAsync(int sellerId, int? status, int? pageSize, int? pageIndex)
        {
            var result = await _valuationService.getPreliminaryValuationByStatusOfSellerAsync(sellerId, status, pageSize, pageIndex);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }


        //approved dung chung cho ca staff, seller update status
        [HttpPut]
        public async Task<IActionResult> UpdateStatusForValuationsAsync(int id, int status)
        {
            var result = await _valuationService.UpdateStatusForValuationsAsync(id, status);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        //reject dung seller update status
        [HttpPut]
        public async Task<IActionResult> RejectForValuationsAsync(int id, int status, string reason)
        {
            var result = await _valuationService.RejectForValuationsAsync(id, status, reason);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> CreateRecieptAsync(int id, ReceiptDTO receipt)
        {
            var result = await _valuationService.CreateRecieptAsync(id, receipt);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> getDetailHistoryValuation(int valuationId)
        {
            var result = await _historyValuationService.getDetailHistoryValuation(valuationId);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }


        //get all dinh gia cuoi cua staff theo valuation co status 6,7,8

        [HttpGet]
        public async Task<IActionResult> getFinalValuationsOfStaffAsync(int staffId, int? pageSize, int? pageIndex)
        {
            var result = await _valuationService.getFinalValuationsOfStaffAsync(staffId, pageSize, pageIndex);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        //get all valuation by appraiserid 
        [HttpGet]
        public async Task<IActionResult> getPreliminaryValuationsOfAppraiserAsync(int appraiserId, int? pageSize, int? pageIndex)
        {
            var result = await _valuationService.getPreliminaryValuationsOfAppraiserAsync(appraiserId, pageSize, pageIndex);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> getFinalValuationsOfAppraiserAsync(int appraiserId, int? pageSize, int? pageIndex)
        {
            var result = await _valuationService.getFinalValuationsOfAppraiserAsync(appraiserId, pageSize, pageIndex);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
