using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Rules.Productivity.Queries
{
    public class GetProductivityRulesPagedQuery
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool? IsActive { get; set; }
        public string? BonusType { get; set; }
    }
}