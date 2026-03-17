using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Milestones.Commands;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Milestones.Handlers
{
    public class ChangeMilestoneRuleStatusHandler
    {
        private readonly IMilestoneRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public ChangeMilestoneRuleStatusHandler(
            IMilestoneRuleRepository repository,
            IUnitOfWork unitOfWork,
            ICacheService cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task HandleAsync(ChangeMilestoneRuleStatusCommand command)
        {
            var rule = await _repository.GetByIdAsync(command.Id);
            if (rule == null)
                throw new Exception("Milestone rule not found.");

            if (command.IsActive)
            {
                if (rule.IsActive)
                    throw new Exception("Milestone rule is already active.");

                var anyOtherActive = (await _repository.GetAllAsync())
                    .Any(r => r.Id != rule.Id
                                && r.IsActive
                                && r.RedmineProjectId == rule.RedmineProjectId
                                && r.MilestoneName == rule.MilestoneName);
                if (anyOtherActive)
                    throw new Exception(
                        $"Another active milestone rule already exists for project {rule.RedmineProjectId} " +
                        $"and milestone '{rule.MilestoneName}'. Deactivate it first.");

                rule.Activate();
            }
            else
            {
                if (!rule.IsActive)
                    throw new Exception("Milestone rule is already inactive.");
                rule.Deactivate();
            }

            _repository.Update(rule);
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveAsync("milestoneRules:all");
            await _cache.RemoveAsync($"milestoneRule:{rule.Id}");
        }
    }
}