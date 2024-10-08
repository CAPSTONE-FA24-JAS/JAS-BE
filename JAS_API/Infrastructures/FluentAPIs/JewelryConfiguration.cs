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
    public class JewelryConfiguration : IEntityTypeConfiguration<Jewelry>
    {
        public void Configure(EntityTypeBuilder<Jewelry> builder)
        {
            builder.HasOne(x => x.Artist)
                .WithMany(x => x.Jewelries)
                .HasForeignKey(x => x.ArtistId);
            builder.HasOne(x => x.Category)
                .WithMany(x => x.Jewelries)
                .HasForeignKey(x => x.CategoryId);
        }
    }
}
