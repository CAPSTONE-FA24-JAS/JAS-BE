using Application.Interfaces;
using Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public interface IUnitOfWork
    {
        public IAccountRepository AccountRepository { get; }
        public IRoleRepository RoleRepository { get; }
        public IBidLimitRepository BidLimitRepository { get; }
        public IImageValuationRepository ImageValuationRepository { get; }
        public IValuationRepository ValuationRepository { get; }
        public IAddressToShipRepository AddressToShipRepository { get; }
        public IWardRepository WardRepository { get; }
        public IDistrictRepository IDistrictRepository { get; }
        public IProvinceRepository ProvininceRepository { get; }
        public IValuationDocumentRepository ValuationDocumentRepository { get; }
        public ICustomerRepository CustomerRepository { get; }
        public IWalletRepository WalletRepository { get; }
        public IAuctionRepository AuctionRepository { get; }
        public IHistoryValuationRepository HistoryValuationRepository { get; }

        public IJewelryRepository JewelryRepository { get; }

        public IKeyCharacteristicRepository KeyCharacteristicRepository { get; }

        public ICategoryRepository CategoryRepository { get; }

        public IArtistRepository ArtistRepository { get; }

        public IImageJewelryRepository ImageJewelryRepository { get; }

        public IKeyCharacteristicsDetailRepository KeyCharacteristicsDetailRepository { get; }

        public IMainDiamondRepository MainDiamondRepository { get; }

        public ISecondDiamondRepository SecondDiamondRepository { get; }

        public IMainShaphieRepository MainShaphieRepository { get; }

        public ISecondaryShaphieRepository SecondaryShaphieRepository{ get; }

        public IDocumentMainDiamondRepository DocumentMainDiamondRepository { get; }

        public IDocumentSecondaryDiamondRepository DocumentSecondaryDiamondRepository { get; }

        public IImageMainDiamondRepository ImageMainDiamondRepository { get; }

        public IImageSecondDiamondRepository ImageSecondDiamondRepository { get; }

        public IDocumentMainShaphieRepository DocumentMainShaphieRepository {  get; }

        public IDocumentSecondaryShaphieRepository DocumentSecondaryShaphieRepository {  get; }

        public IImageMainShaphieRepository ImageMainShaphieRepository {  get; }

        public IImageSecondaryShaphieRepository ImageSecondaryShaphieRepository {  get; }
        public Task<int> SaveChangeAsync();
    }
}
