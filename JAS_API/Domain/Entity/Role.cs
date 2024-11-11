namespace Domain.Entity
{
    public class Role : BaseEntity
    {
        public string? Name { get; set; }

        public virtual IEnumerable<Account> Accounts { get; set;}
    }
}
