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
    public class ValuationConfiguration : IEntityTypeConfiguration<Valuation>
    {
        public void Configure(EntityTypeBuilder<Valuation> builder)
        {
            builder.HasOne(x => x.Seller)
                .WithMany(x => x.SellerValuations)
                .HasForeignKey(x => x.SellerId);
            builder.HasOne(x => x.Staff)
                .WithMany(x => x.StaffValuations)
                .HasForeignKey(x => x.StaffId);
        }
    }
}
