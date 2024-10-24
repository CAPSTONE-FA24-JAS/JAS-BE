using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.FluentAPIs
{
    public class BidPriceConfiguration : IEntityTypeConfiguration<BidPrice>
    {
        public void Configure(EntityTypeBuilder<BidPrice> builder)
        {
            builder.HasOne(x => x.Customer)
                .WithMany(x => x.BidPrices)
                .HasForeignKey(x => x.CustomerId);

            builder.HasOne(x => x.Lot)
                .WithMany(x => x.BidPrices)
                .HasForeignKey(x => x.LotId);
        }
    }
}
