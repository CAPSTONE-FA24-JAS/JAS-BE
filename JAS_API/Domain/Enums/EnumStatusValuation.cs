using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum EnumStatusValuation
    {
        Requested = 0,
        Assigned = 1,
        RequestedPreliminary = 2,
        Preliminary = 3,
        ApprovedPreliminary = 4,
        RecivedJewelry = 5,
        Evaluated = 6,
        ManagerApproved = 7,
        Authorized = 8,
        Rejected = 9   
    }
}
