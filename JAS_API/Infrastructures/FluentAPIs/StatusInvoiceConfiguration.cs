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
    public class StatusInvoiceConfiguration : IEntityTypeConfiguration<StatusInvoice>
    {
        public void Configure(EntityTypeBuilder<StatusInvoice> builder)
        {
            builder.HasOne(x => x.Invoice)
                .WithMany(x => x.StatusInvoices)
                .HasForeignKey(x => x.InvoiceId);
        }
    }
}
