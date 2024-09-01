using Application.Interfaces;
using Application.Repositories;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructures.Repositories
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        private readonly AppDbContext _dbContext;
        public AccountRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        )
            : base(context, timeService, claimsService)
        {
            _dbContext = context;
        }

        public Task<bool> CheckEmailNameExisted(string email) =>
                                                _dbContext.Accounts.AnyAsync(u => u.Email == email);



        public Task<bool> CheckPhoneNumberExisted(string phonenumber) =>
                                                _dbContext.Accounts.AnyAsync(u => u.PhoneNumber == phonenumber);

        public async Task<IEnumerable<Account>> GetAccountsAsync()
        {
            return await _dbContext.Accounts.ToListAsync();
        }

        public async Task<Role> GetRoleNameByRoleId(int RoleId)
        {
            return await _dbContext.Roles.FirstOrDefaultAsync(x => x.Id == RoleId);
        }

        public Task<IEnumerable<Account>> GetSortedAccountAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Account> GetUserByConfirmationToken(string token)
        {
            if (_dbContext == null)
            {
                throw new InvalidOperationException("DbContext is not initialized.");
            }
            var users = _dbContext.Accounts.ToList();
            Console.WriteLine(users);
            var user = await _dbContext.Accounts.SingleOrDefaultAsync(u => u.ConfirmationToken == token);

            if (user == null)
            {
                throw new KeyNotFoundException($"No user found with the provided confirmation token: {token}");
            }

            return user;

        }

        public async Task<Account> GetUserByEmailAndPasswordHash(string email, string passwordHash)
        {
            var account = _dbContext.Accounts.Where(a => a.Email == email 
                                                    && a.PasswordHash == passwordHash).FirstOrDefault();
            if (account == null)
            {
                throw new KeyNotFoundException("No user found with email or pass");
            }
                return account;
        }

        public Task<IEnumerable<Account>> SearchAccountByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Account>> SearchAccountByRoleNameAsync(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}
