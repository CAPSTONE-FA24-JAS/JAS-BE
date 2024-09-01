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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
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
