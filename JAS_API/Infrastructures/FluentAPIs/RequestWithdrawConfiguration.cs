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
    internal class RequestWithdrawConfiguration : IEntityTypeConfiguration<RequestWithdraw>
    {
        public void Configure(EntityTypeBuilder<RequestWithdraw> builder)
        {
            builder.HasOne(x => x.Wallet)
                .WithMany(x => x.RequestWithdraws)
                .HasForeignKey(x => x.WalletId);
        }
    }
}
