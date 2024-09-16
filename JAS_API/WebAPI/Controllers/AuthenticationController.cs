using Application.Interfaces;
using Application.ViewModels.AccountDTOs;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Service;

namespace WebAPI.Controllers
{
    public class AuthenticationController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterAccountDTO registerObject)
        {
            var result = await _authenticationService.RegisterAsync(registerObject);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            
            var isConfirmed = await _authenticationService.ConfirmTokenAsync(email,token);
            if (isConfirmed.IsSuccess)
            {
                return Ok("Email confirmed successfully.");
            }
            else
            {
                return BadRequest(isConfirmed);
            }
        }


        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginAccountDTO loginObject)
        {
            var result = await _authenticationService.LoginAsync(loginObject);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendOTPForForgetPassword(int userId)
        {
            var result = await _authenticationService.SendOTPForgetPassword(userId);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }

        [HttpPut]
        public async Task<IActionResult> VerifyPassword(VerifyPassword verifyPassword)
        {
            var result = await _authenticationService.VerifyPassword(verifyPassword);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }

    }
}
