namespace Domain.Entity
{
    public class Wallet : BaseEntity
    {
        public string? Balance { get; set;}
        public int? CustomerId { get; set;}
        //
        public virtual Customer? Customer { get; set;}
        //public  IEnumerable<WalletTransaction>? WalletTransactions { get; set;}
        public virtual IEnumerable<RequestWithdraw>? RequestWithdraws { get; set;}
    }
}
