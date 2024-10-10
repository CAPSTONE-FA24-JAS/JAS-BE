using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Customer : BaseEntity
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? CitizenIdentificationCard { get; set; }
        public DateTime? IDIssuanceDate { get; set; }
        public DateTime? IDExpirationDate { get; set; }
        public int? AccountId { get; set; }

        public virtual Account? Account { get; set; }
        public virtual IEnumerable<BidLimit>? BidLimits { get; set; }
        public virtual Wallet? Wallet { get; set; }
        public virtual IEnumerable<AddressToShip>? AddressToShips { get; set; }
        public virtual CreditCard? CreditCard { get; set; }
        public virtual IEnumerable<Valuation>? SellerValuations { get; set; }
        public virtual IEnumerable<FollwerArtist>? FollwerArtists { get; set; }
        public virtual IEnumerable<BidPrice>? BidPrices { get; set; }
        public virtual IEnumerable<CustomerLot>? CustomerLots { get; set; }
        public virtual IEnumerable<Watching>? Watchings { get; set; }
        public virtual IEnumerable<Lot>? Lots { get; set; }
        public virtual IEnumerable<Invoice>? Invoices { get; set; }
        public virtual IEnumerable<Valuation>? Seller { get; set; }

    }
}
