using Application.ViewModels.CustomerLotDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.InvoiceDTOs
{
    public class InvoiceDTO
    {
        public int Id { get; set; }
        public string? Status { get; set; }
        public float? TotalPrice { get; set; }
        public string? LinkBillTransaction { get; set; }
        public string? PaymentMethod { get; set; }
        public int? AddressToShipId { get; set; }
        public int? ShipperId { get; set; }
        public DateTime CreationDate { get; set; }
        public MyBidDTO? MyBidDTO { get; set; }
    }

    public class InvoiceDetailDTO: InvoiceDTO
    {
        public int WinnerId { get; set; }
        public string? WinnerName { get; set; }
        public string? WinnerPhone { get; set; }
        public string? WinnerEmail { get; set; }
        public string? LotNumber { get; set; }
        public int LotId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public float? Tax { get;  set; }
        public string? Note { get; set; }
        public string? AddressToShip { get; set; }
        

        public IEnumerable<StatusInvoiceDTO>? StatusInvoiceDTOs { get; set; }
    }


}
