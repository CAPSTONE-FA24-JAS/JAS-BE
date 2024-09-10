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
    public class ProofConfiguration : IEntityTypeConfiguration<Proof>
    {
        public void Configure(EntityTypeBuilder<Proof> builder)
        {
            builder.HasOne(x => x.ProofType)
                .WithMany(x => x.Proofs)
                .HasForeignKey(x => x.ProofTypeId);
            builder.HasOne(x => x.Jewelry)
                .WithOne(x => x.Proof)
                .HasForeignKey<Proof>(x => x.JewelryId);
        }
    }
}
