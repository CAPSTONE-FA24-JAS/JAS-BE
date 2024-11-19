using Application.ServiceReponse;
using Application.ViewModels.CreditCardDTOs;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<Account> GetUserByEmailAndPasswordHash(string email, string passwordHash);

        Task<bool> CheckEmailNameExisted(string email);
        Task<bool> CheckPhoneNumberExisted(string phonenumber);
        Task<bool> CheckConfirmToken(string email, string token);
        Task<Account> GetUserByConfirmationToken(string token);
        Task<IEnumerable<Account>> SearchAccountByNameAsync(string name);
        Task<IEnumerable<Account>> GetAccountsAsync();
        Task<IEnumerable<Account>> SearchAccountByRoleNameAsync(string roleName);
        Task<IEnumerable<Account>> GetSortedAccountAsync();
        Task<Role> GetRoleNameByRoleId(int RoleId);

    }
}
