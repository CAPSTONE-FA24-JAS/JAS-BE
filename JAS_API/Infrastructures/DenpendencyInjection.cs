using Application;
using Application.Interfaces;
using Application.Repositories;
using Application.Services;
using Application.Utils;
using Domain.Entity;
using Google;
using Infrastructures.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace Infrastructures
{
    public static class DenpendencyInjection
    {
        public static IServiceCollection AddInfrastructuresService(this IServiceCollection services, string databaseConnection)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IBidLimitRepository, BidLimitRepository>();
            services.AddScoped<IBidLimitService, BidLimitService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IImageValuationRepository, ImageValuationRepository>();
            services.AddScoped<IValuationRepository, ValuationRepository>();
            
            services.AddScoped<IAddressToShipService,AddressToShipService>();
            services.AddScoped<IAddressToShipRepository, AddressToShipRepository>();
            services.AddScoped<IWardService, WardService>();
            services.AddScoped<IWardRepository, WardRepository>();
            services.AddScoped<IDistrictService, DistrictService>();
            services.AddScoped<IDistrictRepository,DistrictRepository>();
            services.AddScoped<IProvinceService, ProvinceService>();
            services.AddScoped<IProvinceRepository, ProvinceRepository>();
            services.AddScoped<IValuationDocumentRepository, ValuationDocumentRepository>();
            services.AddScoped<IHistoryValuationRepository, HistoryValuationRepository>();
            services.AddScoped<IHistoryValuationService, HistoryValuationService>();
            services.AddScoped<IJewelryRepository, JewelryRepository>();
            
            services.AddScoped<IKeyCharacteristicRepository, KeyCharacterisicRepository>();
            services.AddScoped<IKeyCharacteristicService, KeyCharacteristicService>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IArtistRepository, ArtistRepository>();
            services.AddScoped<IArtistService, ArtistService>();
            services.AddScoped<IImageJewelryRepository,  ImageJewelryRepository>();
            services.AddScoped<IKeyCharacteristicsDetailRepository, KeyCharacteristicsDetailRepository>();
            services.AddScoped<IMainDiamondRepository, MainDiamondRepository>();
            services.AddScoped<ISecondDiamondRepository, SecondDiamondRepository>();
            services.AddScoped<IMainShaphieRepository, MainShaphieRepository>();
            services.AddScoped<ISecondaryShaphieRepository, SecondaryShaphieRepository>();
            services.AddScoped<IDocumentMainDiamondRepository, DocumentMainDiamondRepository>();
            services.AddScoped<IDocumentSecondaryDiamondRepository, DocumentSecondaryDiamondRepository>();
            services.AddScoped<IImageMainDiamondRepository, ImageMainDiamondRepository>();
            services.AddScoped<IImageSecondDiamondRepository, ImageSecondaryDiamondRepository>();
            services.AddScoped<IDocumentMainShaphieRepository,  DocumentMainShaphieRepository>();
            services.AddScoped<IDocumentSecondaryShaphieRepository, DocumentSecondaryShaphieRepository>();
            services.AddScoped<IImageMainShaphieRepository, ImageMainShaphieRepository>();
            services.AddScoped<IImageSecondaryShaphieRepository, ImageSecondaryShaphieRepository>();
            services.AddScoped<ICurrentTime, CurrentTime>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IAuctionService, AuctionService>();
            services.AddScoped<IAuctionRepository, AuctionRepository>();
            
            services.AddScoped<ILotRepository, LotRepository>();
            services.AddScoped<IStaffRepository, StaffRepository>();
            services.AddScoped<ICustomerLotRepository, CustomerLotRepository>();
            services.AddScoped<IWalletTransactionRepository, WalletTransactionRepository>();
            services.AddScoped<IWalletTransactionService, WalletTransactionService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IBidPriceRepository, BidPriceRepository>();
            services.AddScoped<ICustomerLotService, CustomerLotService>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddSingleton<ShareDB>();
            services.AddScoped<IVNPayService, VNPayService>();
            services.AddScoped<IStatusInvoiceRepository, StatusInvoiceRepository>();
            services.AddScoped<IRequestWithdrawRepository, RequestWithdrawRepository>();
            services.AddScoped<IHistoryStatusCustomerLotRepository, HistoryStatusCustomerLotRepository>();
            services.AddScoped<IFeeShipRepository, FeeShipRepository>();
            services.AddScoped<IFoorFeePercentService, FoorFeePercentService>();
            services.AddScoped<IFloorFeePersentRepository, FloorFeePersentRepository>();
            services.AddScoped<IWatchingService, WatchingService>();
            services.AddScoped<IWatchingRepository, WatchingRepository>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IBlogRepository, BlogRepository>();
            services.AddScoped<IImageBlogRepository, ImageBlogRepository>();
           
            services.AddSingleton<HelperValuation>();
            services.AddScoped<IAutoBidRepository, AutoBidRepository>();
            services.AddScoped<IAutoBidService, AutoBidService>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IDashBoardService, DashBoardService>();

            services.AddScoped<ICreditCardRepository, CreditCardRepository>();
            services.AddSingleton<ShareDBForNotification>();
            services.AddDbContext<AppDbContext>(option =>
            {
                option.UseLazyLoadingProxies().UseNpgsql(databaseConnection);
            });
            services.AddAutoMapper(typeof(Mapper).Assembly);
            //services.AddSingleton<UserManager<Account>>();
            return services;

        }
    }
}
