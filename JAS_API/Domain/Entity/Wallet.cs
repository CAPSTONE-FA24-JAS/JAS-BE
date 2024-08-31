namespace Domain.Entity
{
    public class Wallet : BaseEntity
    {
        public string? Balance { get; set;}
        public int? AccountId { get; set;}
        //
        public virtual Account Account { get; set;}
        public virtual IEnumerable<Transaction> Transactions { get; set;}
    }
}
