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
        private readonly IImageValuationRepository _imageValuationRepository;
        private readonly IValuationRepository _valuationRepository;


        public UnitOfWork(AppDbContext dbContext, IAccountRepository accountRepository, IRoleRepository roleRepository,
                                                  IImageValuationRepository imageValuationRepository, IValuationRepository valuationRepository)
        {
            _dbContext = dbContext;
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _imageValuationRepository = imageValuationRepository;
            _valuationRepository = valuationRepository;
        }

        public IAccountRepository AccountRepository => _accountRepository;

        public IRoleRepository RoleRepository => _roleRepository;

        public IImageValuationRepository ImageValuationRepository => _imageValuationRepository;

        public IValuationRepository ValuationRepository => _valuationRepository;

        public async Task<int> SaveChangeAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

    }
}
