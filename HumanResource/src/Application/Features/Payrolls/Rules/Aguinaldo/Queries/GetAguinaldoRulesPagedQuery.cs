using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Rules.Aguinaldo.Queries
{
    public class GetAguinaldoRulesPagedQuery
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool? isActive { get; set; }
        public int? PayMonth { get; set; }

    }
}