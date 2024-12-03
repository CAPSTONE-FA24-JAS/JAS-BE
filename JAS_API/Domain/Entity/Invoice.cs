using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Invoice : BaseEntity
    {
        public string? Status { get; set; }
        public float? Price { get; set; }
        public float? Free { get; set; }
        public float? FeeShip { get; set; }
        public float? TotalPrice { get; set; }
        public string? TransferInvoice { get; set; }
        public string? LinkBillTransaction { get; set; }
        public int? CustomerId { get; set; }
        public int? CustomerLotId { get; set; }
        public string? PaymentMethod { get; set; }
        public int? AddressToShipId { get; set; }
        public int? StaffId { get; set; }
        public int? ShipperId { get; set; }
        public string? Note {get;set;}
        public bool? IsReceiveAtCompany { get; set; }
        //

        public virtual AddressToShip? AddressToShip { get; set; }
        public virtual Customer? Customer { get; set; }  
        public virtual Staff? Staff { get; set; }
        public virtual Staff? Shipper { get; set; }
        public virtual CustomerLot? CustomerLot { get; set; }
        public virtual WalletTransaction? InvoiceOfWalletTransaction { get; set; } 
        public virtual IEnumerable<StatusInvoice>? StatusInvoices { get; set; }
    }
}
