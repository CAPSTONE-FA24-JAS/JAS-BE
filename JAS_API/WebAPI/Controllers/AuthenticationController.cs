using Application.ViewModels.AccountDTO;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class AuthenticationController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterAccountDTO registerObject)
        {
            //var result = await _authenticationService.RegisterAsync(registerObject);

            //if (!result.Success)
            //{
            //    return BadRequest(result);
            //}
            //else
            //{
            //    return Ok(result);
            //}
            return Ok();
        }

    }
}
