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
    public class CustomerLotConfiguration : IEntityTypeConfiguration<CustomerLot>
    {
        public void Configure(EntityTypeBuilder<CustomerLot> builder)
        {
            builder.HasOne(x => x.Customer)
                .WithMany(x => x.CustomerLots)
                .HasForeignKey(x => x.CustomerId);

            builder.HasOne(x => x.Lot)
                .WithMany(x => x.CustomerLots)
                .HasForeignKey(x => x.LotId);
            builder.HasMany(x => x.HistoryStatusCustomerLots)
                .WithOne(x => x.CustomerLot)
                .HasForeignKey(x => x.CustomerLotId);

        }
    }
}
