using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Vacation.Handlers
{
    public class GetVacationRulesPagedHandler
    {
        private readonly IVacationRuleRepository _repository;
    
        public GetVacationRulesPagedHandler(IVacationRuleRepository repository)
        {
            _repository = repository;
        }
    }
}