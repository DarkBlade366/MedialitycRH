using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Rules.Aguinaldo.DTOs;
using Application.Features.Payrolls.Rules.Aguinaldo.Queries;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Aguinaldo.Handlers
{
    public class GetAguinaldoRulesPagedHandler
    {
        private readonly IAguinaldoRuleRepository _repository;
    
        public GetAguinaldoRulesPagedHandler(IAguinaldoRuleRepository repository)
        {
            _repository = repository;
        }
    
        public async Task<PagedResponse<AguinaldoRuleResponse>> HandleAsync(GetAguinaldoRulesPagedQuery query)
        {
            var allRules = await _repository.GetAllAsync();
    
            if (query.isActive.HasValue)
            {
                if (query.isActive.Value)
                {
                    allRules = allRules
                        .Where(x => x.IsActive)
                        .ToList();
                }
                else
                {
                    allRules = allRules
                        .Where(x => !x.IsActive)
                        .ToList();
                }
            }

            if (query.PayMonth.HasValue)
            {
                allRules = allRules
                    .Where(x => x.PayMonth == query.PayMonth.Value)
                    .ToList();
            }
    
            var totalItems = allRules.Count;
    
            var paged = allRules
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