﻿using Application.Interfaces;
using Application.ViewModels.AccountDTOs;
using Application.ViewModels.CreditCardDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> ViewProfile(int Id)
        {
            var result = await _accountService.GetProfileCustomer(Id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchCustomerByName(string name)
        {
            var result = await _accountService.SearchCustomerByName(name);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewListAccount()
        {
            var result = await _accountService.ViewListAccounts();
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
        public async Task<IActionResult> CreateNewStaff(CreateAccountDTO createDTO)
        {
            var result = await _accountService.CreateAccount(createDTO);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFilters()
        {
            var result = _accountService.GetFilter();
            if(result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFilterAccount(int filter)
        {
            var result = await _accountService.FilterAccount(filter);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAccount(int Id)
        {
            var result = await _accountService.DeleteAccount(Id);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPut]
        public async Task<IActionResult> BanAccount(int Id)
        {
            var result = await _accountService.BanAccount(Id);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UnBanAccount(int Id)
        {
            var result = await _accountService.UnBanAccount(Id);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile(int Id, [FromForm] UpdateProfileDTO updateDTO)
        {
            var result = await _accountService.UpdateProfile(Id,updateDTO);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpGet]
        public async Task<IActionResult> FilterAccountByRole(int roleID)
        {
            var result = await _accountService.FilterAccountByRole(roleID);
            if (result.IsSuccess == true)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCreditCardByCustomer(int customerId)
        {
            var result = await _accountService.GetCreditCardByCustomerAsync(customerId);
            return (result.IsSuccess == true)? Ok(result): BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewCreditCard(CreateCreditCardDTO model)
        {
            var result = await _accountService.CreateNewCreditCardAsync(model);
            return (result.IsSuccess == true) ? Ok(result) : BadRequest(result);
        }
    }
}
