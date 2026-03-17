using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.BaseSalary.DTOs;
using Application.Features.Payrolls.Rules.BaseSalary.Queries;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.BaseSalary.Handlers
{
    public class GetBaseSalaryRulesPagedHandler
    {
        private readonly IBaseSalaryRuleRepository _repository;
        private readonly ICacheService _cache;

        public GetBaseSalaryRulesPagedHandler(IBaseSalaryRuleRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<PagedResponse<BaseSalaryRuleResponse>> HandleAsync(GetBaseSalaryRulesPagedQuery query)
        {
            string cacheKey = "baseSalaryRules:all";
            var rules = await _cache.GetAsync<List<BaseSalaryRule>>(cacheKey);
            if (rules == null)
            {
                rules = (await _repository.GetAllAsync())?.ToList() ?? new List<BaseSalaryRule>();
                await _cache.SetAsync(cacheKey, rules, TimeSpan.FromMinutes(10));
            }

            var filtered = rules.AsEnumerable();

            if (query.IsActive.HasValue)
                filtered = filtered.Where(x => x.IsActive == query.IsActive.Value);

            if (query.Role.HasValue)
                filtered = filtered.Where(x => x.Role == query.Role.Value);

            var filteredList = filtered.ToList();
            var totalItems = filteredList.Count;

            var paged = filteredList
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(r => new BaseSalaryRuleResponse
                {
                    Id = r.Id,
                    Role = r.Role.ToString(),
                    Amount = r.Amount,
                    IsActive = r.IsActive
                })
                .ToList();

            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

            return new PagedResponse<BaseSalaryRuleResponse>
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