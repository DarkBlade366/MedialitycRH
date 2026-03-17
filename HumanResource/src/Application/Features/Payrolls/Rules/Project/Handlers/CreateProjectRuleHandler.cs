using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Project.Commands;
using Application.Features.Payrolls.Rules.Project.DTOs;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Projects.Interfaces;

namespace Application.Features.Payrolls.Rules.Project.Handlers
{
    public class CreateProjectRuleHandler
    {
        private readonly IProjectRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectRepository _projectRepository;
        private readonly ICacheService _cache;
    
        public CreateProjectRuleHandler(
            IProjectRuleRepository repository,
            IUnitOfWork unitOfWork,
            IProjectRepository projectRepository,
            ICacheService cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _projectRepository = projectRepository;
            _cache = cache;
        }
    
        public async Task<ProjectRuleResponse> HandleAsync(CreateProjectRuleCommand command)
        {
            var projectExists = await _projectRepository.ExistsAsync(command.RedmineProjectId);
            
            if (!projectExists)
                throw new Exception($"Project with Id {command.RedmineProjectId} does not exist.");

            var allRules = (await _repository.GetAllAsync()).ToList();
        
            var activeRule = allRules
                .FirstOrDefault(r => r.IsActive
                                && r.RedmineProjectId == command.RedmineProjectId);
            if (activeRule != null)
            {
                if (activeRule.BonusAmount == command.BonusAmount)
                    throw new Exception(
                        $"A project rule for project {command.RedmineProjectId} already exists and is active.");
                else
                    throw new Exception(
                        $"There is already an active project rule for project {command.RedmineProjectId} " +
                        $"with bonus {activeRule.BonusAmount:C}. disable it before creating a different one.");
            }

            var inactiveRule = allRules
                .FirstOrDefault(r => !r.IsActive
                                    && r.RedmineProjectId == command.RedmineProjectId);
            if (inactiveRule != null)
            {
                if (inactiveRule.BonusAmount == command.BonusAmount)
                    throw new Exception($"A project rule for project {command.RedmineProjectId} with bonus {command.BonusAmount} already exists but is disabled. Enable it instead of creating a new one.");
            }

            var rule = new ProjectRule(
                command.RedmineProjectId,
                command.BonusAmount);
    
            await _repository.AddAsync(rule);
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveAsync("projectRules:all");
    
            return new ProjectRuleResponse
            {
                Id = rule.Id,
                RedmineProjectId = rule.RedmineProjectId,
                BonusAmount = rule.BonusAmount,
                IsActive = rule.IsActive
            };
        }
    }
}
