using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Staff : BaseEntity
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? AccountId { get; set; }
        //
        public virtual Account? Account { get; set; }
        public virtual IEnumerable<Valuation>? StaffValuations { get; set; }
        public virtual IEnumerable<Invoice>? StaffInvoices { get; set; }
        public virtual IEnumerable<Invoice>? ShipperInvoices { get; set; }
        public virtual IEnumerable<Lot>? Lots { get; set; }
    }
}
