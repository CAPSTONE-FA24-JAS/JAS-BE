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
    public class WatchingConfiguration : IEntityTypeConfiguration<Watching>
    {
        public void Configure(EntityTypeBuilder<Watching> builder)
        {
            builder.HasOne(x => x.Customer)
                .WithMany(x => x.Watchings)
                .HasForeignKey(x => x.CustomerId);

            builder.HasOne(x => x.Jewelry)
                .WithMany(x => x.Watchings)
                .HasForeignKey(x => x.JewelryId);
        }
    }
}
