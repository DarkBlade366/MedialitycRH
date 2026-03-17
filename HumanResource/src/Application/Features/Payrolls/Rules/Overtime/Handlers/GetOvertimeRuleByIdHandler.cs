using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Overtime.DTOs;
using Application.Features.Payrolls.Rules.Overtime.Queries;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Overtime.Handlers
{
    public class GetOvertimeRuleByIdHandler
    {
        private readonly IOvertimeRuleRepository _repository;
        private readonly ICacheService _cache;

        public GetOvertimeRuleByIdHandler(IOvertimeRuleRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<OvertimeRuleResponse?> HandleAsync(GetOvertimeRuleByIdQuery query)
        {
            string cacheKey = $"overtimeRule:{query.Id}";
            var cached = await _cache.GetAsync<OvertimeRuleResponse>(cacheKey);
            if (cached != null)
                return cached;

            var rule = await _repository.GetByIdAsync(query.Id);
            if (rule == null)
                return null;

            var response = new OvertimeRuleResponse
            {
                Id = rule.Id,
                StandardHoursPerPeriod = rule.StandardHoursPerPeriod,
                OvertimeMultiplier = rule.OvertimeMultiplier,
                HourlyRate = rule.HourlyRate,
                IsActive = rule.IsActive
            };

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));
            
            return response;
        }
    }
}