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
    public class MainDiamondConfiguration : IEntityTypeConfiguration<MainDiamond>
    {
        public void Configure(EntityTypeBuilder<MainDiamond> builder)
        {
            builder.HasOne(x => x.Jewelry)
                .WithMany(x => x.MainDiamonds)
                .HasForeignKey(x => x.JewelryId);
        }

    }
    public class DocumentMainDiamondConfiguration : IEntityTypeConfiguration<DocumentMainDiamond>
    {
        public void Configure(EntityTypeBuilder<DocumentMainDiamond> builder)
        {
            builder.HasOne(x => x.MainDiamond)
                .WithMany(x => x.DocumentMainDiamonds)
                .HasForeignKey(x => x.MainDiamondId);
        }
    }
    public class ImageMainDiamondConfiguration : IEntityTypeConfiguration<ImageMainDiamond>
    {
        public void Configure(EntityTypeBuilder<ImageMainDiamond> builder)
        {
            builder.HasOne(x => x.MainDiamond)
                .WithMany(x => x.ImageMainDiamonds)
                .HasForeignKey(x => x.MainDiamondId);
        }
    }
    public class SecondaryDiamondConfiguration : IEntityTypeConfiguration<SecondaryDiamond>
    {
        public void Configure(EntityTypeBuilder<SecondaryDiamond> builder)
        {

            builder.HasOne(x => x.Jewelry)
                .WithMany(x => x.SecondaryDiamonds)
                .HasForeignKey(x => x.JewelryId);
        }
    }
    public class DocumentSecondaryDiamondConfiguration : IEntityTypeConfiguration<DocumentSecondaryDiamond>
    {
        public void Configure(EntityTypeBuilder<DocumentSecondaryDiamond> builder)
        {
            builder.HasOne(x => x.SecondaryDiamond)
                .WithMany(x => x.DocumentSecondaryDiamonds)
                .HasForeignKey(x => x.SecondaryDiamondId);
        }
    }
    public class ImageSecondaryDiamondConfiguration : IEntityTypeConfiguration<ImageSecondaryDiamond>
    {
        public void Configure(EntityTypeBuilder<ImageSecondaryDiamond> builder)
        {
            builder.HasOne(x => x.SecondaryDiamond)
                .WithMany(x => x.ImageSecondaryDiamonds)
                .HasForeignKey(x => x.SecondaryDiamondId);
        }
    }
}
