using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum EnumStatusValuation
    {
        Requested,
        Assigned,
        RequestedPreliminary,
        Preliminary,
        ApprovedPreliminary,
        RecivedJewelry,
        FinalValuated,
        ManagerApproved,
        Authorized,
        RejectedPreliminary
    }
}
