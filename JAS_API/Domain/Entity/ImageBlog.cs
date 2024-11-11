namespace Domain.Entity
{
    public class ImageBlog : BaseEntity
    {
        public string? ImageLink { get; set; }
        public int? BlogId { get; set; }
        public virtual Blog Blog { get; set; }
    }
}
