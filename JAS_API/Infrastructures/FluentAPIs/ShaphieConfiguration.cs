using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructures.FluentAPIs
{
    public class MainShaphieConfiguration : IEntityTypeConfiguration<MainShaphie>
    {
        public void Configure(EntityTypeBuilder<MainShaphie> builder)
        {
            builder.HasOne(x => x.Jewelry)
                .WithMany(x => x.MainShaphies)
                .HasForeignKey(x => x.JewelryId);
        }
    }
    public class DocumentMainShaphieConfiguration : IEntityTypeConfiguration<DocumentMainShaphie>
    {
        public void Configure(EntityTypeBuilder<DocumentMainShaphie> builder)
        {
            builder.HasOne(x => x.MainShaphie)
                .WithMany(x => x.DocumentMainShaphies)
                .HasForeignKey(x => x.ShaphieId);
        }
    }
    public class ImageMainShaphieConfiguration : IEntityTypeConfiguration<ImageMainShaphie>
    {
        public void Configure(EntityTypeBuilder<ImageMainShaphie> builder)
        {
            builder.HasOne(x => x.MainShaphie)
                .WithMany(x => x.ImageMainShaphies)
                .HasForeignKey(x => x.ShaphieId);
        }
    }
    public class SecondaryShaphieConfiguration : IEntityTypeConfiguration<SecondaryShaphie>
    {
        public void Configure(EntityTypeBuilder<SecondaryShaphie> builder)
        {
            builder.HasOne(x => x.Jewelry)
                .WithMany(x => x.SecondaryShaphies)
                .HasForeignKey(x => x.JewelryId);
        }
    }

    public class DocumentSecondaryShaphieConfiguration : IEntityTypeConfiguration<DocumentSecondaryShaphie>
    {
        public void Configure(EntityTypeBuilder<DocumentSecondaryShaphie> builder)
        {
            builder.HasOne(x => x.SecondaryShaphie)
                .WithMany(x => x.DocumentSecondaryShaphies)
                .HasForeignKey(x => x.ShaphieId);
        }
    }
    public class ImageSecondaryShaphieConfiguration : IEntityTypeConfiguration<ImageSecondaryShaphie>
    {
        public void Configure(EntityTypeBuilder<ImageSecondaryShaphie> builder)
        {
            builder.HasOne(x => x.SecondaryShaphie)
                .WithMany(x => x.ImageSecondaryShaphies)
                .HasForeignKey(x => x.ShaphieId);
        }
    }
}
