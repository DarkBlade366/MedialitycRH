using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Rules.Vacation.Queries
{
    public class GetVacationRulesPagedQuery
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}