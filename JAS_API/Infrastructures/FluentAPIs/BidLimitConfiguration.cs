using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructures.FluentAPIs
{
    public class BidLimitConfiguration : IEntityTypeConfiguration<BidLimit>
    {
        public void Configure(EntityTypeBuilder<BidLimit> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Account)
                .WithMany(x => x.BidLimits)
                .HasForeignKey(x => x.AccountId);
        }
    }
}
