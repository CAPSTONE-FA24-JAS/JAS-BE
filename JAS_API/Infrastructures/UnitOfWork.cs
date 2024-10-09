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
        private readonly IAddressToShipRepository _addressToShipRepository;
        private readonly IWardRepository _wardRepository;
        private readonly IDistrictRepository _districtRepository;
        private readonly IProvinceRepository _provinceRepository;
        private readonly IValuationDocumentRepository _valuationDocumentRepository;
        private readonly IHistoryValuationRepository _historyValuationRepository;


        public UnitOfWork(AppDbContext dbContext, IAccountRepository accountRepository, IRoleRepository roleRepository, IBidLimitRepository bidLimitRepository,
                           IImageValuationRepository imageValuationRepository, IAddressToShipRepository addressToShipRepository, IWardRepository wardRepository, 
                           IDistrictRepository districtRepository, IProvinceRepository provinceRepository, IValuationRepository valuationRepository, 
                           IValuationDocumentRepository valuationDocumentRepository, IHistoryValuationRepository historyValuationRepository)
        {
            _dbContext = dbContext;
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _bidLimitRepository = bidLimitRepository;
            _imageValuationRepository = imageValuationRepository;
            _valuationRepository = valuationRepository;
            _addressToShipRepository = addressToShipRepository;
            _wardRepository = wardRepository;
            _districtRepository = districtRepository;
            _provinceRepository = provinceRepository;
            _valuationDocumentRepository = valuationDocumentRepository;
            _historyValuationRepository = historyValuationRepository;
        }

        public IAccountRepository AccountRepository => _accountRepository;
        public IRoleRepository RoleRepository => _roleRepository;
        public IBidLimitRepository BidLimitRepository => _bidLimitRepository;
        public IImageValuationRepository ImageValuationRepository => _imageValuationRepository;
        public IValuationRepository ValuationRepository => _valuationRepository;

        public IAddressToShipRepository AddressToShipRepository => _addressToShipRepository;

        public IWardRepository WardRepository => _wardRepository;

        public IDistrictRepository IDistrictRepository => _districtRepository;

        public IProvinceRepository ProvininceRepository => _provinceRepository;

        public IValuationDocumentRepository ValuationDocumentRepository => _valuationDocumentRepository;

        public IHistoryValuationRepository HistoryValuationRepository => _historyValuationRepository;
        public async Task<int> SaveChangeAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

    }
}
