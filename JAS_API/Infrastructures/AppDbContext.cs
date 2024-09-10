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
        public DbSet<Role> Roles { get; set; }
        public DbSet<BidLimit> BidLimits { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<ImageBlog> ImageBlogs { get; set; }
        public DbSet<Valuation> Valuations { get; set; }
        public DbSet<ImageValuation> ImageValuations { get; set; }
        public DbSet<ValuationDocument> ValuationDocuments { get; set; }
        public DbSet<ValuationDocumentType> ValuationDocumentTypes { get; set; }
        public DbSet<Proof> Proofs { get; set; }
        public DbSet<ProofType> ProofTypes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<KeyCharacteristic> KeyCharacteristics { get; set; }
        public DbSet<KeyCharacteristicDetail> KeyCharacteristicDetails { get; set; }
        public DbSet<Jewelry> Jewelries { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ValuationConfiguration());
            #region insert data
            //Role
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Customer", IsDeleted = false },
                new Role { Id = 2, Name = "Admin", IsDeleted = false }
                );
            #endregion
        }



    }
}
