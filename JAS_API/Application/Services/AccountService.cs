﻿using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.AccountDTOs;
using Application.ViewModels.CreditCardDTOs;
using AutoMapper;
using Azure;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Entity;
using Domain.Enums;
using System.Drawing;
using System.Linq.Expressions;

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
                    account.IsConfirmed = true;
                    account.PasswordHash = HashPassword.HashWithSHA256(createDTO.PasswordHash);
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
                        accounts = await ConvertListAccountDTO(await _unitOfWork.AccountRepository.GetAllAsync(sort: x => x.CreationDate, ascending: false));
                        reponse.Data = accounts;
                        break;
                    case nameof(Domain.Enums.FilterAccount.Oldest_First):
                        reponse.IsSuccess = true;
                        reponse.Message = "Receive Filter Account Successfull";
                        reponse.Code = 200;
                        accounts = await ConvertListAccountDTO(await _unitOfWork.AccountRepository.GetAllAsync(sort: x => x.CreationDate, ascending: true, includes: new Expression<Func<Domain.Entity.Account, object>>[] { x => x.Role, x => x.Customer }));
                        reponse.Data = accounts;
                        break;
                    case nameof(Domain.Enums.FilterAccount.Active):
                        reponse.IsSuccess = true;
                        reponse.Message = "Receive Filter Account Successfull";
                        reponse.Code = 200;
                        accounts = await ConvertListAccountDTO(await _unitOfWork.AccountRepository.GetAllAsync(x => x.Status == true, includes: new Expression<Func<Domain.Entity.Account, object>>[] { x => x.Role, x => x.Customer }));
                        reponse.Data = accounts;
                        break;
                    case nameof(Domain.Enums.FilterAccount.Inactive):
                        reponse.IsSuccess = true;
                        reponse.Message = "Receive Filter Account Successfull";
                        reponse.Code = 200;
                        accounts = await ConvertListAccountDTO(await _unitOfWork.AccountRepository.GetAllAsync(x => x.Status == false, includes: new Expression<Func<Domain.Entity.Account, object>>[] { x => x.Role, x => x.Customer }));
                        reponse.Data = accounts;
                        break;
                    case nameof(Domain.Enums.FilterAccount.A_Z):
                        reponse.IsSuccess = true;
                        reponse.Message = "Receive Filter Account Successfull";
                        reponse.Code = 200;
                        accounts = await ConvertListAccountDTO(await _unitOfWork.AccountRepository.GetAllAsync(sort: x => x.Customer.FirstName, ascending: true, includes: new Expression<Func<Domain.Entity.Account, object>>[] { x => x.Role, x => x.Customer }));
                        reponse.Data = accounts;
                        break;
                    case nameof(Domain.Enums.FilterAccount.Z_A):
                        reponse.IsSuccess = true;
                        reponse.Message = "Receive Filter Account Successfull";
                        reponse.Code = 200;
                        accounts = await ConvertListAccountDTO(await _unitOfWork.AccountRepository.GetAllAsync(sort: x => x.Customer.FirstName, ascending: false, includes: new Expression<Func<Domain.Entity.Account, object>>[] { x => x.Role, x => x.Customer }));
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

        public async Task<APIResponseModel> GetProfileCustomer(int Id)
        {
            var reponse = new APIResponseModel();
            try
            {
                var account = await _unitOfWork.AccountRepository.GetByIdAsync(id: Id,  x => x.IsConfirmed == true, includes: new Expression<Func<Domain.Entity.Account, object>>[] { x => x.Role, x => x.Customer });

                if (account == null)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Not found account in system";
                    reponse.Code = 401;
                }
                else
                {
                    var mapper = _mapper.Map<AccountDTO>(account);
                    reponse.Data = mapper;
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

        public async Task<APIResponseModel> SearchCustomerByName(string Name)
        {
            var reponse = new APIResponseModel();
            try
            {
                var accounts = await _unitOfWork.AccountRepository.GetAllAsync(x => x.Customer.FirstName.Contains(Name)
                                                                       || x.Customer.LastName.Contains(Name)
                                                                       && x.IsConfirmed == true, includes: x => x.Role);
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
            }
            catch(Exception e)
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
                Domain.Entity.Account account = await _unitOfWork.AccountRepository.GetByIdAsync(Id, includes: new Expression<Func<Domain.Entity.Account, object>>[] { x => x.Role, x => x.Customer });
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
                        enity.Customer.ProfilePicture = uploadResult.SecureUrl.AbsoluteUri;
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
                    
                    foreach (var item in accounts)
                    {
                        var mapper = _mapper.Map<AccountDTO>(item);
                        DTOs.Add(mapper);
                    }
                    reponse.IsSuccess = true;
                    reponse.Message = "Retreive List Account Successfull";
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
        internal async Task<List<AccountDTO>> ConvertListAccountDTO(List<Domain.Entity.Account> accounts)
        {
            var DTOs = new List<AccountDTO>();
                foreach (var account in accounts)
                {
                    var mapper = _mapper.Map<AccountDTO>(account);
                    DTOs.Add(mapper);
                }
         
            return DTOs;
        }

        public async Task<APIResponseModel> CheckBidLimit(int customerId)
        {
            var reponse = new APIResponseModel();
            try
            {
                var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId);
                
                    if (customer.PriceLimit == null)
                    {
                        reponse.IsSuccess = false;
                        reponse.Message = "Customer haven't bidlimt, please regiser new bidlimit before join to lot";
                        reponse.Code = 400;
                        return reponse;
                    }
                    else
                    {
                        if(customer.ExpireDate.Value < DateTime.UtcNow) 
                        {
                            reponse.IsSuccess = false;
                            reponse.Message = "Customer have bidlimt, But BidLimit is outtime, Please register new bidlimit";
                            reponse.Code = 400;
                            return reponse;
                        }
                    }
                reponse.IsSuccess = true;
                reponse.Message = "Customer have bidlimit avaiable";
                reponse.Code = 200;

            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.Message = e.Message;
            }
            return reponse;
        }

        public async Task<APIResponseModel> GetCreditCardByCustomerAsync(int customerId)
        {
            var reponse = new APIResponseModel();
            try
            {
                var creditCards = await _unitOfWork.CreditCardRepository.GetAllAsync(x => x.CustomerId == customerId);

                if (!creditCards.Any())
                {
                    reponse.IsSuccess = true;
                    reponse.Message = "Customer haven't credit Card, please add new credit Card";
                    reponse.Code = 200;
                    return reponse;
                }
                else
                {
                    reponse.IsSuccess = true;
                    reponse.Data = _mapper.Map<IEnumerable<ViewCreditCardDTO>>(creditCards);
                    reponse.Message = "Received Successfuly";
                    reponse.Code = 200;
                    return reponse;
                }
            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.Message = e.Message;
                return reponse;
            }
        }

        public async Task<APIResponseModel> CreateNewCreditCardAsync(CreateCreditCardDTO model)
        {
            var reponse = new APIResponseModel();
            try
            {
                var card = _mapper.Map<CreditCard>(model);
                if(card != null)
                {
                    await _unitOfWork.CreditCardRepository.AddAsync(card);
                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        reponse.IsSuccess = true;
                        reponse.Message = "Add Sucessfuly";
                        reponse.Code = 200;
                        return reponse;
                    }
                    else
                    {
                        reponse.IsSuccess = false;
                        reponse.Message = "Received Faild";
                        reponse.Code = 400;
                        return reponse;
                    }
                }
                else
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Mapping Faild, Check Properti Input";
                    reponse.Code = 400;
                    return reponse;
                }
            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.Message = e.Message;
                return reponse;
            }
        }
    }
}
