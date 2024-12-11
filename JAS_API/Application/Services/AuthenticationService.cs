using Application.Commons;
using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.AccountDTOs;
using AutoMapper;
using Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using static System.Net.WebRequestMethods;

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
                    User = accountDTO,
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
                    //await _unitOfWork.CustomerRepository.AddAsync(user.Customer);    
                    var confirmationLink = $"https://api.jas.id.vn/api/Authentication/ConfirmEmail/confirm?token={user.ConfirmationToken}&email={user.Email}";
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

        public async Task<APIResponseModel> SendOTPForgetPassword(int userId)
        {
            var response = new APIResponseModel();
            try
            {
                var user = await _unitOfWork.AccountRepository.GetByIdAsync(userId);
                
                if (user == null)
                {
                    response.IsSuccess = false;
                    response.Message = "user not found";
                    return response;
                }
                var otp = OtpService.GenerateOtp(user.ConfirmationToken);
                var emailSent = await SendEmail.SendEmailOTP(user.Email, otp);
                if (!emailSent)
                {
                    response.IsSuccess = false;
                    response.Message = "Error sending OTP email.";
                    return response;
                }
                else
                {
                    _unitOfWork.AccountRepository.Update(user);
                    var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                    if (isSuccess)
                    {
                        response.IsSuccess = true;
                        response.Message = "Send OTP successfully, Please check email.";
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
                response.ErrorMessages = new List<string> { ex.Message };
            }
            return response;
        }

        public async Task<APIResponseModel> VerifyPassword(VerifyPassword verifyPassword)
        {
            var response = new APIResponseModel();
            try
            {
                if (verifyPassword.NewPassword != verifyPassword.ConfirmNewPassword)
                {
                    response.IsSuccess = false;
                    response.Message = "Password must same confirm password.";
                }
                else
                {
                    var user = await _unitOfWork.AccountRepository.GetByIdAsync(verifyPassword.UserId);
                    var statusResult = OtpService.ValidateOtp(user.ConfirmationToken, verifyPassword.Otp);
                    dynamic validationResult = statusResult;
                    if (!validationResult.status)
                    {
                        response.IsSuccess = false;
                        response.Message = validationResult.msg + validationResult.timeStepMatched;
                        return response;
                    }
                    else
                    {
                        user.PasswordHash = HashPassword.HashWithSHA256(verifyPassword.NewPassword);
                        _unitOfWork.AccountRepository.Update(user);
                        var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                        if (isSuccess)
                        {
                            var userDTO = _mapper.Map<AccountDTO>(user);
                            response.Data = userDTO;
                            response.IsSuccess = true;
                            response.Message = "update password successfully.";
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = "Error saving the account.";
                        }
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
