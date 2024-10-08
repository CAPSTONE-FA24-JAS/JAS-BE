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
    public class AddressToShipConfiguration : IEntityTypeConfiguration<AddressToShip>
    {
        public void Configure(EntityTypeBuilder<AddressToShip> builder)
        {
            builder.HasOne(x => x.Customer)
                .WithMany(x => x.AddressToShips)
                .HasForeignKey(x => x.CustomerId);
            builder.HasOne(x => x.Ward)
                .WithMany(x => x.AddressToShips)
                .HasForeignKey(x => x.WardId);
        }
    }
}
