using Application.ServiceReponse;
using Application.ViewModels.AccountDTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<APIResponseModel> RegisterAsync(RegisterAccountDTO registerAccountDTO);
        public Task<APIResponseModel> ConfirmTokenAsync(string email, string token);
        public Task<APIResponseModel> LoginAsync(LoginAccountDTO loginAccountDTO);
        public Task<APIResponseModel> SendOTPForgetPassword(int userId);
        public Task<APIResponseModel> VerifyPassword(VerifyPassword verifyPassword);
    }
}
