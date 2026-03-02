using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Rules.Overtime.DTOs;
using Application.Features.Payrolls.Rules.Overtime.Queries;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Overtime.Handlers
{
    public class GetOvertimeRulesPagedHandler
    {
        private readonly IOvertimeRuleRepository _repository;
    
        public GetOvertimeRulesPagedHandler(IOvertimeRuleRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<OvertimeRuleResponse>> HandleAsync(GetOvertimeRulesPagedQuery query)
        {
            var allRules = await _repository.GetAllAsync();
    
            if (query.IsActive.HasValue)
                allRules = allRules
                    .Where(r => r.IsActive == query.IsActive.Value)
                    .ToList();
    
            var totalItems = allRules.Count;
    
            var items = allRules
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(r => new OvertimeRuleResponse
                {
                    Id = r.Id,
                    StandardHoursPerPeriod = r.StandardHoursPerPeriod,
                    OvertimeMultiplier = r.OvertimeMultiplier,
                    IsActive = r.IsActive
                })
                .ToList();

            var totalPages =  (int)Math.Ceiling(totalItems / (double)query.PageSize);
    
            return new PagedResponse<OvertimeRuleResponse>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }
    }
}