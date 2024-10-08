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
    public class LotConfiguration : IEntityTypeConfiguration<Lot>
    {
        public void Configure(EntityTypeBuilder<Lot> builder)
        {
            builder.HasOne(x => x.Auction)
                .WithMany(x => x.Lots)
                .HasForeignKey(x => x.AuctionId);

            builder.HasOne(x => x.Jewelry)
                .WithOne(x => x.Lot)
                .HasForeignKey<Lot>(x => x.JewelryId);

            builder.HasOne(x => x.Staff)
                .WithMany(x => x.Lots)
                .HasForeignKey(x => x.StaffId);

            builder.HasOne(x => x.Seller)
                .WithMany(x => x.Lots)
                .HasForeignKey(x => x.SellerId);
        }
    }
}
