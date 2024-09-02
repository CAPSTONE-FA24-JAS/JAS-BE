using Application.Interfaces;
using Application.ViewModels.AccountDTO;
using Microsoft.AspNetCore.Mvc;

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


    }
}
