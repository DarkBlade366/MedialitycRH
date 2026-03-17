using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Milestones.DTOs;
using Application.Features.Payrolls.Rules.Milestones.Queries;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Milestones.Handlers
{
    public class GetMilestoneRuleByIdHandler
    {
        private readonly IMilestoneRuleRepository _repository;
        private readonly ICacheService _cache;

        public GetMilestoneRuleByIdHandler(IMilestoneRuleRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<MilestoneRuleResponse?> HandleAsync(GetMilestoneRuleByIdQuery query)
        {
            string cacheKey = $"milestoneRule:{query.Id}";
            var cached = await _cache.GetAsync<MilestoneRuleResponse>(cacheKey);
            if (cached != null)
                return cached;

            var rule = await _repository.GetByIdAsync(query.Id);
            if (rule == null)
                return null;

            var response = new MilestoneRuleResponse
            {
                Id = rule.Id,
                RedmineProjectId = rule.RedmineProjectId,
                MilestoneName = rule.MilestoneName,
                BonusAmount = rule.BonusAmount,
                IsActive = rule.IsActive
            };

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));
            
            return response;
        }
    }
}