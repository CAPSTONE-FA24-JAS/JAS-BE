using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructures.FluentAPIs
{
    public class ImageBlogConfiguration : IEntityTypeConfiguration<ImageBlog>
    {
        public void Configure(EntityTypeBuilder<ImageBlog> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Blog)
                .WithMany(x => x.ImageBlogs)
                .HasForeignKey(x => x.BlogId);
        }
    }
}
