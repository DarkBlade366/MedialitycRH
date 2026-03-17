using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Deduction.DTOs;
using Application.Features.Payrolls.Rules.Deduction.Queries;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Deduction.Handlers
{
    public class GetDeductionRuleByIdHandler
    {
        private readonly IDeductionRuleRepository _repository;
        private readonly ICacheService _cache;

        public GetDeductionRuleByIdHandler(IDeductionRuleRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<DeductionRuleResponse?> HandleAsync(GetDeductionRuleByIdQuery query)
        {
            string cacheKey = $"deductionRule:{query.Id}";
            var cached = await _cache.GetAsync<DeductionRuleResponse>(cacheKey);
            if (cached != null)
                return cached;

            var rule = await _repository.GetByIdAsync(query.Id);
            if (rule == null)
                return null;

            var response = new DeductionRuleResponse
            {
                Id = rule.Id,
                Description = rule.Description,
                Percentage = rule.Percentage,
                Type = rule.Type.ToString(),
                IsActive = rule.IsActive
            };

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));
            
            return response;
        }
    }
}