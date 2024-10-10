namespace Domain.Entity
{
    public class Wallet : BaseEntity
    {
        public Decimal? Balance { get; set;}
        public int? CustomerId { get; set;}
        public string? Status { get; set;}
        //
        public virtual Customer? Customer { get; set;}
        public virtual IEnumerable<RequestWithdraw>? RequestWithdraws { get; set;}
    }
}
