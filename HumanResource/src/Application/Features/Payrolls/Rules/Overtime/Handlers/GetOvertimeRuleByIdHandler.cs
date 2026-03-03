using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Overtime.DTOs;
using Application.Features.Payrolls.Rules.Overtime.Queries;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Overtime.Handlers
{
    public class GetOvertimeRuleByIdHandler
    {
        private readonly IOvertimeRuleRepository _repository;
    
        public GetOvertimeRuleByIdHandler(IOvertimeRuleRepository repository)
        {
            _repository = repository;
        }

        public async Task<OvertimeRuleResponse?> HandleAsync(GetOvertimeRuleByIdQuery query)
        {
            var rule = await _repository.GetByIdAsync(query.Id);

            if (rule is null)
                return null;

            return new OvertimeRuleResponse
            {
                Id = rule.Id,
                StandardHoursPerPeriod = rule.StandardHoursPerPeriod,
                OvertimeMultiplier = rule.OvertimeMultiplier,
                HourlyRate = rule.HourlyRate,
                IsActive = rule.IsActive
            };
        }
    }
}