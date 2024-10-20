using Application;
using Application.Repositories;
using Infrastructures.Repositories;
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
        private readonly ICustomerRepository _customerRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IAuctionRepository _auctionRepository;
        private readonly IHistoryValuationRepository _historyValuationRepository;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IKeyCharacteristicRepository _keyCharacteristicRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IArtistRepository _artistRepository;
        private readonly IImageJewelryRepository _imageJewelryRepository;
        private readonly IKeyCharacteristicsDetailRepository _keyCharacteristicsDetailRepository;
        private readonly IMainDiamondRepository _mainDiamondRepository;
        private readonly ISecondDiamondRepository _secondDiamondRepository;
        private readonly IMainShaphieRepository _mainShaphieRepository;
        private readonly ISecondaryShaphieRepository _secondaryShaphieRepository;
        private readonly IDocumentMainDiamondRepository _documentMainDiamondRepository;
        private readonly IDocumentSecondaryDiamondRepository _documentSecondaryDiamondRepository;
        private readonly IImageMainDiamondRepository _imageMainDiamondRepository;
        private readonly IImageSecondDiamondRepository _imageSecondDiamondRepository;
        private readonly IDocumentMainShaphieRepository _documentMainShaphieRepository;
        private readonly IDocumentSecondaryShaphieRepository _documentSecondaryShaphieRepository;
        private readonly IImageMainShaphieRepository _imageMainShaphieRepository;
        private readonly IImageSecondaryShaphieRepository _imageSecondaryShaphieRepository;
        private readonly ILotRepository _lotRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly ICustomerLotRepository _customerLotRepository;
        private readonly IBidPriceRepository _bidPriceRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;

        public UnitOfWork(AppDbContext dbContext, IAccountRepository accountRepository, IRoleRepository roleRepository,

                          IBidLimitRepository bidLimitRepository, IImageValuationRepository imageValuationRepository, 
                          IAddressToShipRepository addressToShipRepository, IWardRepository wardRepository, 
                          IDistrictRepository districtRepository, IProvinceRepository provinceRepository, 
                          IValuationRepository valuationRepository, IValuationDocumentRepository valuationDocumentRepository, 
                          ICustomerRepository customerRepository,IWalletRepository walletRepository,
                          IAuctionRepository auctionRepository, IHistoryValuationRepository historyValuationRepository,
                          IJewelryRepository jewelryRepository, IKeyCharacteristicRepository keyCharacteristicRepository,
                          ICategoryRepository categoryRepository, IArtistRepository artistRepository,
                          IImageJewelryRepository imageJewelryRepository, IKeyCharacteristicsDetailRepository keyCharacteristicsDetailRepository,
                          IMainDiamondRepository mainDiamondRepository, ISecondDiamondRepository secondDiamondRepository,
                          IMainShaphieRepository mainShaphieRepository, ISecondaryShaphieRepository secondaryShaphieRepository,
                          IDocumentMainDiamondRepository documentMainDiamondRepository, IDocumentSecondaryDiamondRepository documentSecondaryDiamondRepository,
                          IImageMainDiamondRepository imageMainDiamondRepository, IImageSecondDiamondRepository imageSecondDiamondRepository,
                          IDocumentMainShaphieRepository documentMainShaphieRepository, IDocumentSecondaryShaphieRepository documentSecondaryShaphieRepository,
                          IImageMainShaphieRepository imageMainShaphieRepository, IImageSecondaryShaphieRepository imageSecondaryShaphieRepository,
                          ILotRepository lotRepository, IStaffRepository staffRepository, ICustomerLotRepository customerLotRepository,
                          IBidPriceRepository bidPriceRepository, IWalletTransactionRepository walletTransactionRepository)
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
            _customerRepository = customerRepository;
            _walletRepository = walletRepository;
            _auctionRepository = auctionRepository;
            _historyValuationRepository = historyValuationRepository;
            _jewelryRepository = jewelryRepository;
            _keyCharacteristicRepository = keyCharacteristicRepository;
            _categoryRepository = categoryRepository;
            _artistRepository = artistRepository;
            _imageJewelryRepository = imageJewelryRepository;
            _keyCharacteristicsDetailRepository = keyCharacteristicsDetailRepository;
            _mainDiamondRepository = mainDiamondRepository;
            _secondDiamondRepository = secondDiamondRepository;
            _mainShaphieRepository = mainShaphieRepository;
            _secondaryShaphieRepository = secondaryShaphieRepository;
            _documentMainDiamondRepository = documentMainDiamondRepository;
            _documentSecondaryDiamondRepository = documentSecondaryDiamondRepository;
            _imageMainDiamondRepository = imageMainDiamondRepository;
            _imageSecondDiamondRepository = imageSecondDiamondRepository;
            _documentMainShaphieRepository = documentMainShaphieRepository;
            _documentSecondaryShaphieRepository = documentSecondaryShaphieRepository;
            _imageMainShaphieRepository = imageMainShaphieRepository;
            _imageSecondaryShaphieRepository = imageSecondaryShaphieRepository;
            _lotRepository = lotRepository; 
            _staffRepository = staffRepository;
            _customerLotRepository = customerLotRepository;

            _bidPriceRepository = bidPriceRepository;
            _walletTransactionRepository = walletTransactionRepository;
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
        public ICustomerRepository CustomerRepository => _customerRepository;
        public IWalletRepository WalletRepository => _walletRepository;
        public IAuctionRepository AuctionRepository => _auctionRepository;
        public IHistoryValuationRepository HistoryValuationRepository => _historyValuationRepository;
        public IJewelryRepository JewelryRepository => _jewelryRepository;
        public IKeyCharacteristicRepository KeyCharacteristicRepository => _keyCharacteristicRepository;   
        public ICategoryRepository CategoryRepository => _categoryRepository;

        public IArtistRepository ArtistRepository => _artistRepository;

        public IImageJewelryRepository ImageJewelryRepository => _imageJewelryRepository;

        public IKeyCharacteristicsDetailRepository KeyCharacteristicsDetailRepository => _keyCharacteristicsDetailRepository;

        public IMainDiamondRepository MainDiamondRepository => _mainDiamondRepository;

        public ISecondDiamondRepository SecondDiamondRepository => _secondDiamondRepository;

        public IMainShaphieRepository MainShaphieRepository => _mainShaphieRepository;

        public ISecondaryShaphieRepository SecondaryShaphieRepository => _secondaryShaphieRepository;

        public IDocumentMainDiamondRepository DocumentMainDiamondRepository => _documentMainDiamondRepository;

        public IDocumentSecondaryDiamondRepository DocumentSecondaryDiamondRepository => _documentSecondaryDiamondRepository;

        public IImageMainDiamondRepository ImageMainDiamondRepository => _imageMainDiamondRepository;

        public IImageSecondDiamondRepository ImageSecondDiamondRepository => _imageSecondDiamondRepository;

        public IDocumentMainShaphieRepository DocumentMainShaphieRepository => _documentMainShaphieRepository;

        public IDocumentSecondaryShaphieRepository DocumentSecondaryShaphieRepository => _documentSecondaryShaphieRepository;

        public IImageMainShaphieRepository ImageMainShaphieRepository => _imageMainShaphieRepository;

        public IImageSecondaryShaphieRepository ImageSecondaryShaphieRepository => _imageSecondaryShaphieRepository;

        public ILotRepository LotRepository => _lotRepository;

        public IStaffRepository StaffRepository => _staffRepository;

        public ICustomerLotRepository CustomerLotRepository => _customerLotRepository;

        public IBidPriceRepository BidPriceRepository => _bidPriceRepository;

        public IWalletTransactionRepository WalletTransactionRepository => _walletTransactionRepository;

        public async Task<int> SaveChangeAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

    }
}
