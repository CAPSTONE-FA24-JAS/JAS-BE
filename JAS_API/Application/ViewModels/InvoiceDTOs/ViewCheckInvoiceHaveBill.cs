using Microsoft.AspNetCore.Http;


namespace Application.ViewModels.InvoiceDTOs
{
    public class ViewCheckInvoiceHaveBill
    {
        public int Id { get; set; }
        public string? Status { get; set; }
        public float? Price { get; set; }
        public float? Free { get; set; }
        public float? FeeShip { get; set; }
        public float? TotalPrice { get; set; }
        public string? TransferInvoice { get; set; }
        public string? LinkBillTransaction { get; set; }
        public int? CustomerId { get; set; }
        public int? CustomerLotId { get; set; }
        public int? PaymentMethodId { get; set; }
        public int? AddressToShipId { get; set; }
        public int? StaffId { get; set; }
        public int? ShipperId { get; set; }
        public string? Note { get; set; }
    }
}
