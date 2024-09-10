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
    public class ValuationDocumentConfiguration : IEntityTypeConfiguration<ValuationDocument>
    {
        public void Configure(EntityTypeBuilder<ValuationDocument> builder)
        {
            builder.HasOne(x => x.Valuation)
                .WithMany(x => x.ValuationDocuments)
                .HasForeignKey(x => x.ValuationId);
            builder.HasOne(x => x.ValuationDocumentType)
                .WithMany(x => x.ValuationDocuments)
                .HasForeignKey(x => x.ValuationDocumentTypeId);
        }
    }
}
