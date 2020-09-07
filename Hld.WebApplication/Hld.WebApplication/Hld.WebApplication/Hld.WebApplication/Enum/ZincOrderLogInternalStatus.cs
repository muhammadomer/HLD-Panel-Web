﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.Enum
{
    public enum ZincOrderLogInternalStatus
    {
        InProcess,
        Error,
        Shipped,
        [Display(Name ="InProgress (Success)")]
        InProgressSuccess,
        OrderRequestSent,
        Processed_OutSideZinc,
        Canceled

    }
}
