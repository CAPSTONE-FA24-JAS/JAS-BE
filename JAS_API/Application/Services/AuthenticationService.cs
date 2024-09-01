using Application.Commons;
using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.AccountDTO;
using AutoMapper;
using Domain.Entity;
using System.Data.Common;

namespace Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthenticationService(IUnitOfWork unitOfWork, AppConfiguration configuration, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
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
                    // Tạo token ngẫu nhiên
                    user.ConfirmationToken = Guid.NewGuid().ToString();

                    user.RoleId = 1;
                    await _unitOfWork.AccountRepository.AddAsync(user);
                    var confirmationLink = $"https://localhost:7251/swagger/confirm?token={user.ConfirmationToken}";

                    // Gửi email xác nhận
                    var emailSent = await SendEmail.SendConfirmationEmail(user.Email, confirmationLink);
                    if (!emailSent)
                    {
                        // Xử lý khi gửi email không thành công
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
