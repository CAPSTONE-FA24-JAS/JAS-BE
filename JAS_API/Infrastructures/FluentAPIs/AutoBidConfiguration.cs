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
    public class AutoBidConfiguration : IEntityTypeConfiguration<AutoBid>
    {
        public void Configure(EntityTypeBuilder<AutoBid> builder)
        {
            builder.HasOne(x => x.CustomerLot)
                .WithMany(x => x.AutoBids)
                .HasForeignKey(x => x.CustomerLotId);
                
        }
    }
}
