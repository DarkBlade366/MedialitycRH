using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Vacation.DTOs;
using Application.Features.Payrolls.Rules.Vacation.Queries;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Vacation.Handlers
{
    public class GetVacationRuleByIdHandler
    {
        private readonly IVacationRuleRepository _repository;
        private readonly ICacheService _cache;

        public GetVacationRuleByIdHandler(IVacationRuleRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<VacationRuleResponse?> HandleAsync(GetVacationRuleByIdQuery query)
        {
            string cacheKey = $"vacationRule:{query.Id}";
            var cached = await _cache.GetAsync<VacationRuleResponse>(cacheKey);
            if (cached != null)
                return cached;

            var rule = await _repository.GetByIdAsync(query.Id);
            if (rule == null)
                return null;

            var response = new VacationRuleResponse
            {
                Id = rule.Id,
                AccrualRatePerMonth = rule.AccrualRatePerMonth,
                IsActive = rule.IsActive
            };

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));
            
            return response;
        }
    }
}