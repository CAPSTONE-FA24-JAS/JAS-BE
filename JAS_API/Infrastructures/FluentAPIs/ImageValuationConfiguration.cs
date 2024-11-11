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
    public class ImageValuationConfiguration : IEntityTypeConfiguration<ImageValuation>
    {
        public void Configure(EntityTypeBuilder<ImageValuation> builder)
        {
            builder.HasOne(x => x.Valuation)
                .WithMany(x => x.ImageValuations)
                .HasForeignKey(x => x.ValuationId);
        }
    }
}
