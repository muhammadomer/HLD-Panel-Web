using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.Enum
{
    public enum DropShipStatusTypes
    {
        None = 0,
        Pending = 1,
        Requested = 2,
        Acknowledged = 3,
        Processed = 4,
        PartialProcessed = 5,
        Cancelled = 6,
        Invalid = 7
    }
}
