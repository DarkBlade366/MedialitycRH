using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Rules.Aguinaldo.Queries
{
    public class GetAguinaldoRulesPagedQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool? isActive { get; set; }
        public int? PayMonth { get; set; }

    }
}