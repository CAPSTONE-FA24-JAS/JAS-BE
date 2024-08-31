using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructures.FluentAPIs
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.Blogs)
                .WithOne(x => x.Account);
            builder.HasOne(x => x.Role)
                .WithMany(x => x.Accounts)
                .HasForeignKey(x => x.RoleId);
            builder.HasOne(x => x.BidLimit)
                .WithOne(x => x.Account);
            builder.HasOne(x => x.Wallet)
                .WithOne(x => x.Account);
        }
    }
}
