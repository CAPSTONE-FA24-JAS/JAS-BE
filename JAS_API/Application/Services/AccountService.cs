using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.AccountDTO;
using AutoMapper;
using Azure;
using Domain.Entity;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
                var account = _mapper.Map<Account>(createDTO);
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
                        accounts = _mapper.Map<IEnumerable<AccountDTO>>(await _unitOfWork.AccountRepository.GetAllAsync(sort: x => x.CreationDate, ascending: true));
                        reponse.Data = accounts;
                        break;
                    case nameof(Domain.Enums.FilterAccount.Active):
                        reponse.IsSuccess = true;
                        reponse.Message = "Receive Filter Account Successfull";
                        reponse.Code = 200;
                        accounts = _mapper.Map<IEnumerable<AccountDTO>>(await _unitOfWork.AccountRepository.GetAllAsync(x => x.Status == true));
                        reponse.Data = accounts;
                        break;
                    case nameof(Domain.Enums.FilterAccount.Inactive):
                        reponse.IsSuccess = true;
                        reponse.Message = "Receive Filter Account Successfull";
                        reponse.Code = 200;
                        accounts = _mapper.Map<IEnumerable<AccountDTO>>(await _unitOfWork.AccountRepository.GetAllAsync(x => x.Status == false));
                        reponse.Data = accounts;
                        break;
                    case nameof(Domain.Enums.FilterAccount.A_Z):
                        reponse.IsSuccess = true;
                        reponse.Message = "Receive Filter Account Successfull";
                        reponse.Code = 200;
                        accounts = _mapper.Map<IEnumerable<AccountDTO>>(await _unitOfWork.AccountRepository.GetAllAsync(sort: x => x.FirstName, ascending: true));
                        reponse.Data = accounts;
                        break;
                    case nameof(Domain.Enums.FilterAccount.Z_A):
                        reponse.IsSuccess = true;
                        reponse.Message = "Receive Filter Account Successfull";
                        reponse.Code = 200;
                        accounts = _mapper.Map<IEnumerable<AccountDTO>>(await _unitOfWork.AccountRepository.GetAllAsync(sort: x => x.FirstName, ascending: false));
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
                var account = await _unitOfWork.AccountRepository.GetByIdAsync(Id, x => x.IsConfirmed == true);
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
                                                                       &&  x.IsConfirmed == true,
                                                                       x => x.Role);
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

        public Task<APIResponseModel> UpdateProfile(int Id, UpdateProfileDTO updateDTO)
        {
            throw new NotImplementedException();
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
    }
}
