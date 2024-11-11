namespace Domain.Entity
{
    public class Account : BaseEntity
    {
        
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public bool? Status { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ConfirmationToken { get; set; }
        public bool? IsConfirmed { get; set; }
        public int? RoleId { get; set; }

        //Enity Relationship
        public virtual Role? Role { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual Staff? Staff { get; set; }
        public virtual IEnumerable<Blog>? Blogs { get; set; }
        public virtual IEnumerable<Notification>? Notifications { get; set; }
    }
}
