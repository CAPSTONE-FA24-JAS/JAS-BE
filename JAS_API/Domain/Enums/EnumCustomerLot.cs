using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum EnumCustomerLot
    {
        Registed = 1,
        CreateInvoice = 2,
        PendingPayment = 3,
        Paid = 4,
        Delivering = 5,
        Delivered =6,
        Rejected = 7,
        Finished = 8,
        Refunded = 9,
        Cancelled = 10,
        Closed = 11


    }
}
