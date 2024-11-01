using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Infrastructures.FluentAPIs;
namespace Infrastructures
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AddressToShip> AddressToShips { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<AutoBid> AutoBids { get; set; }
        public DbSet<BidIncrement> BidIncrements { get; set; }
        public DbSet<BidLimit> BidLimits { get; set; }
        public DbSet<BidPrice> BidPrices { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerLot> CustomerLots { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<DocumentMainDiamond> DocumentMainDiamonds { get; set; }
        public DbSet<DocumentMainShaphie> DocumentMainShaphies { get; set; }
        public DbSet<DocumentSecondaryDiamond> DocumentSecondaryDiamonds { get; set; }
        public DbSet<DocumentSecondaryShaphie> DocumentSecondaryShaphies { get; set; }
        public DbSet<FeeShip> FeeShips { get; set; }
        public DbSet<FloorFeePersent> FloorFeePersents { get; set; }
        public DbSet<FollwerArtist> FollwerArtists { get; set; }
        public DbSet<HistoryValuation> HistoryValuations { get; set; }
        public DbSet<ImageBlog> ImageBlogs { get; set; }
        public DbSet<ImageJewelry> ImageJewelrys { get; set; }
        public DbSet<ImageMainDiamond> ImageMainDiamonds { get; set; }
        public DbSet<ImageMainShaphie> ImageMainShaphies { get; set; }
        public DbSet<ImageSecondaryDiamond> ImageSecondaryDiamonds { get; set; }
        public DbSet<ImageSecondaryShaphie> ImageSecondaryShaphies { get; set; }
        public DbSet<ImageValuation> ImageValuations { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Jewelry> Jewelries { get; set; }
        public DbSet<KeyCharacteristic> KeyCharacteristics { get; set; }
        public DbSet<KeyCharacteristicDetail> KeyCharacteristicDetails { get; set; }
        public DbSet<Lot> Lots { get; set; }
        public DbSet<MainDiamond> MainDiamonds { get; set; }
        public DbSet<MainShaphie> MainShaphies { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<RequestWithdraw> RequestWithdraws { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SecondaryDiamond> SecondaryDiamonds { get; set; }
        public DbSet<SecondaryShaphie> SecondaryShaphies { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<StatusInvoice> StatusInvoices { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Valuation> Valuations { get; set; }
        public DbSet<ValuationDocument> ValuationDocuments { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<Watching> Watchings { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ValuationConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceConfiguration());
            modelBuilder.ApplyConfiguration(new ImageBlogConfiguration());
            #region insert data
            //Role
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Customer", IsDeleted = false },
                new Role { Id = 2, Name = "Manager", IsDeleted = false }
                //new Role { Id = 3, Name = "Staff", IsDeleted = false },
                //new Role { Id = 4, Name = "Appraiser", IsDeleted = false },
                //new Role { Id = 5, Name = "Admin", IsDeleted = false },
                //new Role { Id = 5, Name = "Shipper", IsDeleted = false }
                );
            #endregion


        }
    }
}
