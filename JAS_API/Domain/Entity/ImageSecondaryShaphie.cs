namespace Domain.Entity
{
    public class ImageSecondaryShaphie : BaseEntity
    {
        public string? ImageLink { get; set; }
        public int? SecondaryShaphieId { get; set; }
        //
        public virtual SecondaryShaphie? SecondaryShaphie { get; set; }
    }
}
