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
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.HasOne(x => x.AddressToShip)
                .WithMany(x => x.Invoices)
                .HasForeignKey(x => x.AddressToShipId);

            builder.HasOne(x => x.CustomerLot)
                .WithOne(x => x.Invoice)
                .HasForeignKey<Invoice>(x => x.CustomerLotId);

            builder.HasOne(x => x.Customer)
                .WithMany(x => x.Invoices)
                .HasForeignKey(x => x.CustomerId);

            builder.HasOne(x => x.Staff)
                .WithMany(x => x.StaffInvoices)
                .HasForeignKey(x => x.StaffId);

            builder.HasOne(x => x.Shipper)
                .WithMany(x => x.ShipperInvoices)
                .HasForeignKey(x => x.ShipperId);
        }
    }
}
