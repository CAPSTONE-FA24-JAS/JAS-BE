using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.AccountDTOs;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Entity;
using Domain.Enums;
using System.Drawing;

namespace Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private const string Tags = "Backend_ImageProfile";

        public AccountService(IUnitOfWork unitOfWork, IMapper mapper, Cloudinary cloudinary)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinary = cloudinary;
        }

        public async Task<APIResponseModel> BanAccount(int Id)
        {
            var reponse = new APIResponseModel();
            try
            {
                var accountExits = await _unitOfWork.AccountRepository.GetByIdAsync(Id);
                if(accountExits == null)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Not Found Account For Ban";
                    reponse.Code = 401;
                }
                //baned
                if(accountExits.Status == false)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Account is baned, cant ban again";
                    reponse.Code = 401;
                }
                accountExits.Status = false;
                _unitOfWork.AccountRepository.Update(accountExits);
                if (await _unitOfWork.SaveChangeAsync() > 0)
                {
                    reponse.IsSuccess = true;
                    reponse.Message = "Baned Successfull";
                    reponse.Code = 200;
                }
            }
            catch (Exception ex)
            {
                reponse.IsSuccess = false;
                reponse.Message = ex.Message;
            }
            return reponse;
        }

        public async Task<APIResponseModel> CreateAccount(CreateAccountDTO createDTO)
        {
            var reponse = new APIResponseModel();
            try
            {
                var account = _mapper.Map<Domain.Entity.Account>(createDTO);
                if (account == null)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Mapper Faild";
                }
                else
                {
                    account.Status = true;
                    await _unitOfWork.AccountRepository.AddAsync(account);
                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        reponse.IsSuccess = true;
                        reponse.Message = "Create new Account Succsessfull";
                    }
                    else
                    {
                        reponse.IsSuccess = false;
                        reponse.Message = "Faild when saving";
                    }
                }

            }catch (Exception ex)
            {
                reponse.IsSuccess = false;
                reponse.Message = ex.Message;
            }
            return reponse;
        }

        public async Task<APIResponseModel> DeleteAccount(int Id)
        {
            var reponse = new APIResponseModel();
            try
            {
                var checkexit = await _unitOfWork.AccountRepository.GetByIdAsync(Id); 
                //check acocunt có trong order hoặc giao dịch nào chưa ...
                //chưa thì delete
                if(checkexit == null)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Not found account for delete";
                    reponse.Code = 401;
                }
                _unitOfWork.AccountRepository.SoftRemove(checkexit);
                if(await _unitOfWork.SaveChangeAsync() > 0)
                {
                    reponse.IsSuccess = true;
                    reponse.Message = "Deleted account successfull";
                    reponse.Code = 200;
                }
            }catch(Exception e)
            {
                reponse.IsSuccess = false;
                reponse.Message = e.Message;
            }
            return reponse;
        }

        public async Task<APIResponseModel> FilterAccount(int filter)
        {
            var reponse = new APIResponseModel();
            try
            {
                var filters = EnumHelper.GetEnums<FilterAccount>();
                var f = filters.FirstOrDefault(x => x.Value == filter).Name;
                IEnumerable<AccountDTO> accounts = Enumerable.Empty<AccountDTO>();
                switch (f)
                {
                    case nameof(Domain.Enums.FilterAccount.Newest_First):
                        reponse.IsSuccess = true;
                        reponse.Message = "Receive Filter Account Successfull";
                        reponse.Code = 200;
                        accounts =  _mapper.Map<IEnumerable<AccountDTO>>(await _unitOfWork.AccountRepository.GetAllAsync(sort: x => x.CreationDate, ascending: false));
                        reponse.Data = accounts;
                        break;
                    case nameof(Domain.Enums.FilterAccount.Oldest_First):
                        reponse.IsSuccess = true;
                        reponse.Message = "Receive Filter Account Successfull";
                        reponse.Code = 200;
                        accounts = _mapper.Map<IEnumerable<AccountDTO>>(await _unitOfWork.AccountRepository.GetAllAsync(sort: x => x.CreationDate, ascending: true, includes: x => x.Role));
                        reponse.Data = accounts;
                        break;
                    case nameof(Domain.Enums.FilterAccount.Active):
                        reponse.IsSuccess = true;
                        reponse.Message = "Receive Filter Account Successfull";
                        reponse.Code = 200;
                        accounts = _mapper.Map<IEnumerable<AccountDTO>>(await _unitOfWork.AccountRepository.GetAllAsync(x => x.Status == true, includes: x => x.Role));
                        reponse.Data = accounts;
                        break;
                    case nameof(Domain.Enums.FilterAccount.Inactive):
                        reponse.IsSuccess = true;
                        reponse.Message = "Receive Filter Account Successfull";
                        reponse.Code = 200;
                        accounts = _mapper.Map<IEnumerable<AccountDTO>>(await _unitOfWork.AccountRepository.GetAllAsync(x => x.Status == false, includes: x => x.Role));
                        reponse.Data = accounts;
                        break;
                    case nameof(Domain.Enums.FilterAccount.A_Z):
                        reponse.IsSuccess = true;
                        reponse.Message = "Receive Filter Account Successfull";
                        reponse.Code = 200;
                        accounts = _mapper.Map<IEnumerable<AccountDTO>>(await _unitOfWork.AccountRepository.GetAllAsync(sort: x => x.FirstName, ascending: true, includes: x => x.Role));
                        reponse.Data = accounts;
                        break;
                    case nameof(Domain.Enums.FilterAccount.Z_A):
                        reponse.IsSuccess = true;
                        reponse.Message = "Receive Filter Account Successfull";
                        reponse.Code = 200;
                        accounts = _mapper.Map<IEnumerable<AccountDTO>>(await _unitOfWork.AccountRepository.GetAllAsync(sort: x => x.FirstName, ascending: false, includes: x => x.Role));
                        reponse.Data = accounts;
                        break;
                    default:
                        reponse.IsSuccess = false;
                        reponse.Message = "Receive Filter Account Fail";
                        reponse.Code = 400;
                        break;
                } 
            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.Message = e.Message;
            }
            return reponse;
        }

        public async Task<APIResponseModel> GetFilter()
        {
            var reponse = new APIResponseModel();
            try
            {
                var filters = EnumHelper.GetEnums<FilterAccount>();
                if (filters == null)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Receive Filter Account Fail";
                    reponse.Code = 400;
                }
                else
                {
                    reponse.IsSuccess = true;
                    reponse.Message = "Receive Filter Account Successfull";
                    reponse.Code = 200;
                    reponse.Data = filters;
                }
            }
            catch (Exception e)
            {
                reponse.IsSuccess = true;
                reponse.Message = e.Message;
            }
            return reponse;
        }

        public async Task<APIResponseModel> GetProfileAccount(int Id)
        {
            var reponse = new APIResponseModel();
            try
            {
                var account = await _unitOfWork.AccountRepository.GetByIdAsync(Id, x => x.IsConfirmed == true, includes: x => x.Role);
                if(account == null)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Not found account in system";
                    reponse.Code = 401;
                }
                else
                {
                    reponse.Data = _mapper.Map<AccountDTO>(account);
                    reponse.IsSuccess = true;
                    reponse.Message = "Retrive profile successfully";
                    reponse.Code = 200;
                }
            }catch(Exception e)
            {
                reponse.IsSuccess = false;
                reponse.Message = e.Message;
            }
            return reponse;
        }

        public async Task<APIResponseModel> SearchAccountByName(string Name)
        {
            var reponse = new APIResponseModel();
            try
            {
                var accounts = await _unitOfWork.AccountRepository.GetAllAsync(x => x.FirstName.Contains(Name) 
                                                                       || x.LastName.Contains(Name) 
                                                                       &&  x.IsConfirmed == true, includes: x => x.Role);
                var DTOs = new List<AccountDTO>();
                if (accounts == null)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "List Account Null";
                    reponse.Code = 400;
                }
                else
                {
                    reponse.IsSuccess = true;
                    reponse.Message = "Search List Account Successfull";
                    reponse.Code = 200;
                    foreach (var item in accounts)
                    {
                        var mapper = _mapper.Map<AccountDTO>(item);
                        mapper.RoleName = item.Role.Name;
                        DTOs.Add(mapper);
                    }
                    reponse.Data = DTOs;
                }
            }catch(Exception e)
            {
                reponse.IsSuccess = false;
                reponse.Message = e.Message;
            }
            return reponse;
        }

        public async Task<APIResponseModel> UnBanAccount(int Id)
        {
            var reponse = new APIResponseModel();
            try
            {
                var accountExits = await _unitOfWork.AccountRepository.GetByIdAsync(Id);
                if (accountExits == null)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Not Found Account For Ban";
                    reponse.Code = 401;
                }
                //unban
                if (accountExits.Status == true)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Account is UnBaned, cant ban again";
                    reponse.Code = 401;
                }
                accountExits.Status = true;
                _unitOfWork.AccountRepository.Update(accountExits);
                if(await _unitOfWork.SaveChangeAsync() > 0)
                {
                    reponse.IsSuccess = true;
                    reponse.Message = "UnBaned Successfull";
                    reponse.Code = 200;
                }
            }
            catch (Exception ex)
            {
                reponse.IsSuccess = false;
                reponse.Message = ex.Message;
            }
            return reponse;
        }

        public async Task<APIResponseModel> UpdateProfile(int Id, UpdateProfileDTO updateDTO)
        {
            var reponse = new APIResponseModel();
            try
            {
                Domain.Entity.Account account = await _unitOfWork.AccountRepository.GetByIdAsync(Id, includes: x => x.Role);
                if (account == null)
                {
                    reponse.Message = $"Not Found Account By Id:"+Id+".";
                    reponse.Code = 400;
                    reponse.IsSuccess = false;
                }
                else
                {
                    var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams
                    {
                        File = new FileDescription(updateDTO.ProfileImage.FileName,
                                                   updateDTO.ProfileImage.OpenReadStream()),
                        Tags = Tags
                    }).ConfigureAwait(false);

                    if (uploadResult == null || uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        reponse.Message = $"Image upload failed."+ uploadResult.Error.Message+ "";
                        reponse.Code = (int)uploadResult.StatusCode;
                        reponse.IsSuccess = false;
                    }
                    else
                    {
                        var enity = _mapper.Map(updateDTO, account);
                        enity.ProfilePicture = uploadResult.SecureUrl.AbsoluteUri;
                        _unitOfWork.AccountRepository.Update(enity);
                        if (await _unitOfWork.SaveChangeAsync() > 0)
                        {
                            reponse.Message = $"Image upload Successfull";
                            reponse.Code = 200;
                            reponse.IsSuccess = true;
                        }
                    }
                }
            }
            catch(Exception ex) {
                reponse.ErrorMessages = ex.Message.Split(',').ToList();
                reponse.Message = "Exception";
                reponse.Code = 500;
                reponse.IsSuccess = false;
            }
            return reponse;
        }

        public async Task<APIResponseModel> ViewListAccounts()
        {
            var reponse = new APIResponseModel();
            try
            {
                var accounts = await _unitOfWork.AccountRepository.GetAllAsync(x => x.IsConfirmed == true, includes :x => x.Role);
                var DTOs = new List<AccountDTO>();
                if (accounts == null)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "List Account Null";
                    reponse.Code = 400;
                }
                else
                {
                    reponse.IsSuccess = true;
                    reponse.Message = "Retreive List Account Successfull";
                    foreach (var item in accounts)
                    {
                        var mapper = _mapper.Map<AccountDTO>(item);
                        mapper.RoleName = item.Role.Name;
                        DTOs.Add(mapper);
                    }
                    reponse.Data = DTOs;
                }
            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.Message = e.Message;
            }
            return reponse;
        }

        public async Task<APIResponseModel> FilterAccountByRole(int role)
        {
            var reponse = new APIResponseModel();
            try
            {
                var accounts = await _unitOfWork.AccountRepository.GetAllAsync(condition: x => x.RoleId == role, includes: x => x.Role);
                List<AccountDTO> DTOs = new List<AccountDTO>();
                if(accounts == null || accounts.Count == 0)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "List account is null";
                    reponse.Code = 400;
                }
                foreach(var account in accounts)
                {
                    var mapper = _mapper.Map<AccountDTO>(account);
                    mapper.RoleName = account.Role.Name;
                    DTOs.Add(mapper);
                    reponse.IsSuccess = true;
                    reponse.Message = "Received list account successfull";
                    reponse.Code = 200;
                    reponse.Data = DTOs;
                }
            }catch(Exception e)
            {
                reponse.IsSuccess = false;
                reponse.ErrorMessages = new List<string> { e.InnerException.Message.ToString() };
            }
            return reponse;
        }
    }
}
