using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Project.Commands;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Project.Handlers
{
    public class ChangeProjectRuleStatusHandler
    {
        private readonly IProjectRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;
    
        public ChangeProjectRuleStatusHandler(
            IProjectRuleRepository repository,
            IUnitOfWork unitOfWork,
            ICacheService cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }
    
        public async Task HandleAsync(ChangeProjectRuleStatusCommand command)
        {
            var rule = await _repository.GetByIdAsync(command.Id);
    
            if (rule is null)
                throw new Exception("Project rule not found.");
    
            if (command.IsActive)
            {
                if (rule.IsActive)
                    throw new Exception("Project rule is already active.");

                var anyOtherActive = (await _repository.GetAllAsync())
                    .Any(r => r.Id != rule.Id
                                && r.IsActive
                                && r.RedmineProjectId == rule.RedmineProjectId);
                if (anyOtherActive)
                    throw new Exception(
                        $"Another active project rule already exists for project {rule.RedmineProjectId}. " +
                        "deactivate it first.");

                rule.Activate();
            }
            else
            {
                if (!rule.IsActive)
                    throw new Exception("Project rule is already inactive.");
                    
                rule.Deactivate();
            }
    
            _repository.Update(rule);
    
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveAsync("projectRules:all");
            await _cache.RemoveAsync($"projectRule:{rule.Id}");
        }
    }
}
