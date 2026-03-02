using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Productivity.DTOs;
using Application.Features.Payrolls.Rules.Productivity.Queries;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Productivity.Handlers
{
    public class GetProductivityRuleByIdHandler
    {
        private readonly IProductivityRuleRepository _repository;
    
        public GetProductivityRuleByIdHandler(IProductivityRuleRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProductivityRuleResponse?> HandleAsync(GetProductivityRuleByIdQuery query)
        {
            var rule = await _repository.GetByIdAsync(query.Id);
    
            if (rule is null)
                return null;
    
            return new ProductivityRuleResponse
            {
                Id = rule.Id,
                MinimumTarget = rule.MinimumTarget,
                FullBonusTarget = rule.FullBonusTarget,
                BonusValue = rule.BonusValue,
                BonusType = rule.BonusType.ToString(),
                MaxBonusCap = rule.MaxBonusCap,
                IsActive = rule.IsActive
            };
        }
    }
}