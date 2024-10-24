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
    public class FollwerArtistConfiguration : IEntityTypeConfiguration<FollwerArtist>
    {
        public void Configure(EntityTypeBuilder<FollwerArtist> builder)
        {
            builder.HasOne(x => x.Customer)
                .WithMany(x => x.FollwerArtists)
                .HasForeignKey(x => x.CustomerId);

            builder.HasOne(x => x.Artist)
                .WithMany(x => x.FollwerArtists)
                .HasForeignKey(x => x.ArtistId);
        }
    }
}
