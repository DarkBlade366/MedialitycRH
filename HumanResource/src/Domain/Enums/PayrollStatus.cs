using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum PayrollStatus
    {
        Draft = 1,
        UnderReview = 2,
        Approved = 3,
        Closed = 4
    }
}