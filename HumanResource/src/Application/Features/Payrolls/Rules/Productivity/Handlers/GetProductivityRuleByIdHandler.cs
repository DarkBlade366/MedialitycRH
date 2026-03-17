using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Productivity.DTOs;
using Application.Features.Payrolls.Rules.Productivity.Queries;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Productivity.Handlers
{
    public class GetProductivityRuleByIdHandler
    {
        private readonly IProductivityRuleRepository _repository;
        private readonly ICacheService _cache;

        public GetProductivityRuleByIdHandler(IProductivityRuleRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<ProductivityRuleResponse?> HandleAsync(GetProductivityRuleByIdQuery query)
        {
            string cacheKey = $"productivityRule:{query.Id}";
            var cached = await _cache.GetAsync<ProductivityRuleResponse>(cacheKey);
            if (cached != null)
                return cached;

            var rule = await _repository.GetByIdAsync(query.Id);
            if (rule == null)
                return null;

            var response = new ProductivityRuleResponse
            {
                Id = rule.Id,
                MinimumTarget = rule.MinimumTarget,
                FullBonusTarget = rule.FullBonusTarget,
                BonusValue = rule.BonusValue,
                BonusType = rule.BonusType.ToString(),
                MaxBonusCap = rule.MaxBonusCap,
                IsActive = rule.IsActive
            };

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));
            
            return response;
        }
    }
}