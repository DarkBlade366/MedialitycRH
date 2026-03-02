using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Deduction.Handlers
{
    public class GetDeductionRulesPagedHandler
    {
        private readonly IDeductionRuleRepository _repository;
    
        public GetDeductionRulesPagedHandler(IDeductionRuleRepository repository)
        {
            _repository = repository;
        }
    }
}