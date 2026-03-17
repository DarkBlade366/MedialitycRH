using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Aguinaldo.DTOs;
using Application.Features.Payrolls.Rules.Aguinaldo.Queries;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Aguinaldo.Handlers
{
    public class GetAguinaldoRulesPagedHandler
    {
        private readonly IAguinaldoRuleRepository _repository;
        private readonly ICacheService _cache;

        public GetAguinaldoRulesPagedHandler(IAguinaldoRuleRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<PagedResponse<AguinaldoRuleResponse>> HandleAsync(GetAguinaldoRulesPagedQuery query)
        {
            string cacheKey = "aguinaldoRules:all";
            var allRules = await _cache.GetAsync<List<AguinaldoRule>>(cacheKey);
            if (allRules == null)
            {
                allRules = (await _repository.GetAllAsync())?.ToList() ?? new List<AguinaldoRule>();
                await _cache.SetAsync(cacheKey, allRules, TimeSpan.FromMinutes(10));
            }

            var filtered = allRules.AsEnumerable();

            if (query.isActive.HasValue)
                filtered = filtered.Where(x => x.IsActive == query.isActive.Value);

            if (query.PayMonth.HasValue)
                filtered = filtered.Where(x => x.PayMonth == query.PayMonth.Value);

            var filteredList = filtered.ToList();
            var totalItems = filteredList.Count;

            var paged = filteredList
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(r => new AguinaldoRuleResponse
                {
                    Id = r.Id,
                    MonthlyAccrualPercentage = r.MonthlyAccrualPercentage,
                    PayMonth = r.PayMonth,
                    IsActive = r.IsActive
                })
                .ToList();

            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

            return new PagedResponse<AguinaldoRuleResponse>
            {
                Items = paged,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }
    }
}