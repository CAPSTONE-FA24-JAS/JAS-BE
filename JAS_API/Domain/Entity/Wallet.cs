namespace Domain.Entity
{
    public class Wallet : BaseEntity
    {
        public Decimal? Balance { get; set;}
        public Decimal? AvailableBalance { get; set;}
        public Decimal? FrozenBalance { get; set; }
        public int? CustomerId { get; set;}
        public string? Status { get; set;}
        public string? Password { get; set;}
        //
        public virtual Customer? Customer { get; set;}
        public virtual IEnumerable<RequestWithdraw>? RequestWithdraws { get; set;}
        public virtual IEnumerable<WalletTransaction>? WalletTransactions { get; set; }
    }
}
