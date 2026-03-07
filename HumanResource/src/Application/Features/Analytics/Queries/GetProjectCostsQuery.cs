using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Analytics.Queries
{
    public class GetProjectCostsQuery
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int? ProjectId { get; set; }
    }
}