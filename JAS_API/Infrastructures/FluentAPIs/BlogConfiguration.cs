using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructures.FluentAPIs
{
    public class BlogConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.ImageBlogs)
                .WithOne(x => x.Blog);
            builder.HasOne(x => x.Account)
                .WithMany(x => x.Blogs)
                .HasForeignKey(x => x.AccountId);
        }
    }
}
