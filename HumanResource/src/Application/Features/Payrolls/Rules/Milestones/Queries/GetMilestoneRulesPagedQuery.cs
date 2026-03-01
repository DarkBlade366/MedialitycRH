using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Rules.Milestones.Queries
{
    public class GetMilestoneRulesPagedQuery
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool? isActive { get; set; }
        public int? ProjectId { get; set; } 
    }
}