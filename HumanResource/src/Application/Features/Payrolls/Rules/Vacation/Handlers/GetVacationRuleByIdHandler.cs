using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Vacation.Handlers
{
    public class GetVacationRuleByIdHandler
    {
        private readonly IVacationRuleRepository _repository;
    
        public GetVacationRuleByIdHandler(IVacationRuleRepository repository)
        {
            _repository = repository;
        }
    }
}