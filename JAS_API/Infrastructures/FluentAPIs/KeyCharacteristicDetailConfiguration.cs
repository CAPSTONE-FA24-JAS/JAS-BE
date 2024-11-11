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
    public class KeyCharacteristicDetailConfiguration : IEntityTypeConfiguration<KeyCharacteristicDetail>
    {
        public void Configure(EntityTypeBuilder<KeyCharacteristicDetail> builder)
        {
            builder.HasOne(x => x.Jewelry)
                .WithMany(x => x.KeyCharacteristicDetails)
                .HasForeignKey(x => x.JewelryId);
            builder.HasOne(x => x.KeyCharacteristic)
                .WithMany(x => x.KeyCharacteristicDetails)
                .HasForeignKey(x => x.KeyCharacteristicId);
        }
    }
}
