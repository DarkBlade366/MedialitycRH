using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Rules.Deduction.Queries
{
    public class GetDeductionRulesPagedQuery
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}