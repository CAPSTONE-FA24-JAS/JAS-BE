namespace Domain.Entity
{
    public class WalletTransaction : BaseEntity
    {
        public int? DocNo { get; set; }
        public float? Amount { get; set; }
        public DateTime? TransactionTime { get; set; }
        public string? Status { get; set; }
        public int? WalletId { get; set; }

        //
        //public  Wallet? Wallet { get; set; }
        //public  Invoice? Invoice { get; set; }
        //public  CustomerLot? CustomerLot { get; set; }
        //public  RequestWithdraw? RequestWithdraw { get; set; }
    }
}
