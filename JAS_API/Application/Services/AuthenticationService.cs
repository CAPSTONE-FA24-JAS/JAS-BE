using Application.Commons;
using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.AccountDTO;
using AutoMapper;
using Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using System.Data.Common;

namespace Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ICurrentTime _currentTime;

        public AuthenticationService(IUnitOfWork unitOfWork, AppConfiguration configuration, IMapper mapper, ICurrentTime currentTime)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
            _currentTime = currentTime;
        }

        public async Task<APIResponseModel> LoginAsync(LoginAccountDTO loginAccountDTO)
        {
            var response = new APIResponseModel();
            try
            {
                var hashPassword = Utils.HashPassword.HashWithSHA256(loginAccountDTO.Password);
                var account = await _unitOfWork.AccountRepository.GetUserByEmailAndPasswordHash(loginAccountDTO.Email, hashPassword);
                if (account == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Email or password is invalid!";
                    return response;
                }
                var token = account.GenerateJsonWebToken(
                    _configuration,
                    _configuration.JWTSection.SecretKey,
                    _currentTime.GetCurrentTime()
                    );

                var accountDTO = _mapper.Map<AccountDTO>( account );
                var authResponse = new LoginResponseDTO
                {
                    Account = accountDTO,
                    AccessToken = token
                };
                response.IsSuccess = true;
                response.Message = "Login successfully.";
                response.Data = authResponse;
            }
            catch (Exception ex) 
            {
                response.IsSuccess = false;
                response.Message = "Error";
                response.ErrorMessages = new List<string> { ex.Message };

            }
            return response;
        }

        public async Task<APIResponseModel> ConfirmTokenAsync(string email, string tokenconfirm)
        {
            var response = new APIResponseModel();
            try
            {
                var emailChecked = await _unitOfWork.AccountRepository.CheckEmailNameExisted(email);
                if (!emailChecked)
                {
                    response.IsSuccess = false;
                    response.Message = "Email does not exist.";
                    return response;
                }

                var users= await _unitOfWork.AccountRepository.GetAllAsync();
                var user = users.FirstOrDefault(x => x.Email == email);
                if (user == null)
                {
                    response.IsSuccess = false;
                    response.Message = "User not found.";
                    return response;
                }

                var result = await _unitOfWork.AccountRepository.CheckConfirmToken(user.Email, tokenconfirm);
                if (!result)
                {
                    response.IsSuccess = false;
                    response.Message = "Token failed, not confirmed.";
                }
                else
                {
                    user.IsConfirmed = true;
                    _unitOfWork.AccountRepository.Update(user);
                    if(await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        response.IsSuccess = true;
                        response.Message = "Confirm successful.";
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "Update faild, not confirmed.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }


        public async Task<APIResponseModel> RegisterAsync(RegisterAccountDTO registerAccountDTO)
        {
            var response = new APIResponseModel();
            try
            {
                var exist = await _unitOfWork.AccountRepository.CheckEmailNameExisted(registerAccountDTO.Email);
                if (exist)
                {
                    response.IsSuccess = false;
                    response.Message = "Email is existed";
                    return response;
                }
                
                    var user = _mapper.Map<Account>(registerAccountDTO);
                    user.ConfirmationToken = Guid.NewGuid().ToString();
                    user.RoleId = 1;
                    user.Status = true;
                    user.PasswordHash = HashPassword.HashWithSHA256(registerAccountDTO.PasswordHash);
                    await _unitOfWork.AccountRepository.AddAsync(user);
                    var confirmationLink = $"http://localhost:7251/api/Authentication/ConfirmEmail/confirm?token={user.ConfirmationToken}&email={user.Email}";
                
                    var emailSent = await SendEmail.SendConfirmationEmail(user.Email, confirmationLink);
                    if (!emailSent)
                    {
                        response.IsSuccess = false;
                        response.Message = "Error sending confirmation email.";
                        return response;
                    }
                    else
                    {
                        var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                        if (isSuccess)
                        {
                            var userDTO = _mapper.Map<AccountDTO>(user);
                            response.Data = userDTO;
                            response.IsSuccess = true;
                            response.Message = "Register successfully.";
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = "Error saving the account.";
                        }
                    }
            }
            catch (DbException ex)
            {
                response.IsSuccess = false;
                response.Message = "Database error occurred.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error";
            }
            return response;
        }
    }
}
