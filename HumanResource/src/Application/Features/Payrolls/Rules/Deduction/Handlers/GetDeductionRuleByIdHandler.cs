using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Deduction.Handlers
{
    public class GetDeductionRuleByIdHandler
    {
        private readonly IDeductionRuleRepository _repository;
    
        public GetDeductionRuleByIdHandler(IDeductionRuleRepository repository)
        {
            _repository = repository;
        }
    }
}