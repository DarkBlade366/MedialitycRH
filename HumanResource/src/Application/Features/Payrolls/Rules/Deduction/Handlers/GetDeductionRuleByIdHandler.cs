using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Deduction.DTOs;
using Application.Features.Payrolls.Rules.Deduction.Queries;
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

        public async Task<DeductionRuleResponse?> HandleAsync(GetDeductionRuleByIdQuery query)
        {
            var rule = await _repository.GetByIdAsync(query.Id);
    
            if (rule is null)
                return null;
    
            return new DeductionRuleResponse
            {
                Id = rule.Id,
                Description = rule.Description,
                Percentage = rule.Percentage,
                Type = rule.Type.ToString(),
                IsActive = rule.IsActive
            };
        }
    }
}