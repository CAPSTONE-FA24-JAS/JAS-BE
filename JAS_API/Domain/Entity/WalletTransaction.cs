namespace Domain.Entity
{
    public class WalletTransaction : BaseEntity
    {
        public string? transactionId { get; set; }
        public string? transactionType { get; set; } 
        public int? DocNo { get; set; }
        public float? Amount { get; set; }
        public DateTime? TransactionTime { get; set; }
        public int transactionPerson { get; set; }
        public string? Status { get; set; }
       
    }
}
