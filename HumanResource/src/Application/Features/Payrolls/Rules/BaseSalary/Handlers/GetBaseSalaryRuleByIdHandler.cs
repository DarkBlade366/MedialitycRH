using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.BaseSalary.DTOs;
using Application.Features.Payrolls.Rules.BaseSalary.Queries;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.BaseSalary.Handlers
{
    public class GetBaseSalaryRuleByIdHandler
    {
        private readonly IBaseSalaryRuleRepository _repository;
    
        public GetBaseSalaryRuleByIdHandler(IBaseSalaryRuleRepository repository)
        {
            _repository = repository;
        }
    
        public async Task<BaseSalaryRuleResponse?> HandleAsync(GetBaseSalaryRuleByIdQuery query)
        {
            var rule = await _repository.GetByIdAsync(query.Id);
    
            if (rule is null)
                return null;
    
            return new BaseSalaryRuleResponse
            {
                Id = rule.Id,
                Role = rule.Role.ToString(),
                Amount = rule.Amount,
                IsActive = rule.IsActive
            };
        }
    }
}