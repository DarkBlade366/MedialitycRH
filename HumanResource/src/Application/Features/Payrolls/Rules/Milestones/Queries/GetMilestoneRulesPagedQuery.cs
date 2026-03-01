using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Rules.Milestones.Queries
{
    public class GetMilestoneRulesPagedQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool? isActive { get; set; } = true;
        public int? ProjectId { get; set; } 
    }
}