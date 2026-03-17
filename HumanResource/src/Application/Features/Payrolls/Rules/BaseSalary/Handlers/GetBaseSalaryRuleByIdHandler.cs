using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.BaseSalary.DTOs;
using Application.Features.Payrolls.Rules.BaseSalary.Queries;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.BaseSalary.Handlers
{
    public class GetBaseSalaryRuleByIdHandler
    {
        private readonly IBaseSalaryRuleRepository _repository;
        private readonly ICacheService _cache;

        public GetBaseSalaryRuleByIdHandler(IBaseSalaryRuleRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<BaseSalaryRuleResponse?> HandleAsync(GetBaseSalaryRuleByIdQuery query)
        {
            string cacheKey = $"baseSalaryRule:{query.Id}";
            var cached = await _cache.GetAsync<BaseSalaryRuleResponse>(cacheKey);
            if (cached != null)
                return cached;

            var rule = await _repository.GetByIdAsync(query.Id);
            if (rule == null)
                return null;

            var response = new BaseSalaryRuleResponse
            {
                Id = rule.Id,
                Role = rule.Role.ToString(),
                Amount = rule.Amount,
                IsActive = rule.IsActive
            };

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));
            
            return response;
        }
    }
}