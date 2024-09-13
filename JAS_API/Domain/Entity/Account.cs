namespace Domain.Entity
{
    public class Account : BaseEntity
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? PasswordHash { get; set; }
        public bool? Status { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ConfirmationToken { get; set; }
        public bool? IsConfirmed { get; set; }
        public string? VNPayAccount { get; set; }
        public string? VNPayBankCode { get; set; }
        public string? VNPayAccountName { get; set; }
        public string? CitizenIdentificationCard { get; set; }
        public DateTime? IDIssuanceDate { get; set; }
        public DateTime? IDExpirationDate { get; set; }
        public int? RoleId { get; set; }

        //Enity Relationship
        public virtual Role? Role { get; set; }
        public virtual IEnumerable<Blog>? Blogs { get; set; }
        public virtual IEnumerable<BidLimit>? BidLimits { get; set; }
        public virtual Wallet? Wallet { get; set; }
        public virtual IEnumerable<Valuation>? SellerValuations { get; set; }
        public virtual IEnumerable<Valuation>? StaffValuations { get; set; }

    }
}
