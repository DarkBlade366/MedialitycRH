using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Aguinaldo.DTOs;
using Application.Features.Payrolls.Rules.Aguinaldo.Queries;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Aguinaldo.Handlers
{
    public class GetAguinaldoRuleByIdHandler
    {
        private readonly IAguinaldoRuleRepository _repository;
        private readonly ICacheService _cache;

        public GetAguinaldoRuleByIdHandler(IAguinaldoRuleRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<AguinaldoRuleResponse?> HandleAsync(GetAguinaldoRuleByIdQuery query)
        {
            string cacheKey = $"aguinaldoRule:{query.Id}";
            var cached = await _cache.GetAsync<AguinaldoRuleResponse>(cacheKey);
            if (cached != null)
                return cached;

            var rule = await _repository.GetByIdAsync(query.Id);
            if (rule == null)
                return null;

            var response = new AguinaldoRuleResponse
            {
                Id = rule.Id,
                MonthlyAccrualPercentage = rule.MonthlyAccrualPercentage,
                PayMonth = rule.PayMonth,
                IsActive = rule.IsActive
            };

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));

            return response;
        }
    }
}