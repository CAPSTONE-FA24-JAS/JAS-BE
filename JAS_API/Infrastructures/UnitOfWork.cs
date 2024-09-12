using Application;
using Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private readonly IAccountRepository _accountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IBidLimitRepository _bidLimitRepository;

        public UnitOfWork(AppDbContext dbContext, IAccountRepository accountRepository, IRoleRepository roleRepository, IBidLimitRepository bidLimitRepository)
        {
            _dbContext = dbContext;
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _bidLimitRepository = bidLimitRepository;
        }

        public IAccountRepository AccountRepository => _accountRepository;

        public IRoleRepository RoleRepository => _roleRepository;

        public IBidLimitRepository BidLimitRepository => _bidLimitRepository;

        public async Task<int> SaveChangeAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

    }
}
