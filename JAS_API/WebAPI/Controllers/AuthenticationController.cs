using Application.Interfaces;
using Application.ViewModels.AccountDTOs;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Service;

namespace WebAPI.Controllers
{
    public class AuthenticationController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly OtpService _otpService;

        public AuthenticationController(IAuthenticationService authenticationService, OtpService otpService)
        {
            _authenticationService = authenticationService;
            _otpService = otpService;
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
        public async Task<IActionResult> ForgetPassword(int userId, string email,string newPassword)
        {
            var result = await _authenticationService.ForgetPassword(userId, email, newPassword,_otpService.GenerateOtp());

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
