using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Vacation.DTOs;
using Application.Features.Payrolls.Rules.Vacation.Queries;
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

        public async Task<VacationRuleResponse?> HandleAsync(GetVacationRuleByIdQuery query)
        {
            var rule = await _repository.GetByIdAsync(query.Id);

            if (rule == null) 
                return null;
    
            return new VacationRuleResponse
            {
                Id = rule.Id,
                AccrualRatePerMonth = rule.AccrualRatePerMonth,
                PayVacationOnUse = rule.PayVacationOnUse,
                IsActive = rule.IsActive
            };
        }
    }
}