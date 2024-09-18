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
        private readonly IImageValuationRepository _imageValuationRepository;
        private readonly IValuationRepository _valuationRepository;
        private readonly IValuationDocumentRepository _valuationDocumentRepository;
        
        public UnitOfWork(AppDbContext dbContext, IAccountRepository accountRepository, IRoleRepository roleRepository, 
                                                 IBidLimitRepository bidLimitRepository,IImageValuationRepository imageValuationRepository, 
                                                 IValuationRepository valuationRepository, IValuationDocumentRepository valuationDocumentRepository)
        {
            _dbContext = dbContext;
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _bidLimitRepository = bidLimitRepository;
            _imageValuationRepository = imageValuationRepository;
            _valuationRepository = valuationRepository;
            _valuationDocumentRepository = valuationDocumentRepository;
        }

        public IAccountRepository AccountRepository => _accountRepository;
        public IRoleRepository RoleRepository => _roleRepository;
        public IBidLimitRepository BidLimitRepository => _bidLimitRepository;
        public IImageValuationRepository ImageValuationRepository => _imageValuationRepository;
        public IValuationRepository ValuationRepository => _valuationRepository;

        public IValuationDocumentRepository ValuationDocumentRepository => _valuationDocumentRepository;
        public async Task<int> SaveChangeAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

    }
}
