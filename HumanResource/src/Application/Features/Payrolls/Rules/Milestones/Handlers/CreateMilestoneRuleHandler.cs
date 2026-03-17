using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Milestones.Commands;
using Application.Features.Payrolls.Rules.Milestones.DTOs;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Projects.Interfaces;

namespace Application.Features.Payrolls.Rules.Milestones.Handlers
{
    public class CreateMilestoneRuleHandler
    {
        private readonly IMilestoneRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectMilestoneRepository _projectMilestoneRepository;
        private readonly ICacheService _cache;

        public CreateMilestoneRuleHandler(
            IMilestoneRuleRepository repository,
            IUnitOfWork unitOfWork,
            IProjectRepository projectRepository,
            IProjectMilestoneRepository projectMilestoneRepository,
            ICacheService cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _projectRepository = projectRepository;
            _projectMilestoneRepository = projectMilestoneRepository;
            _cache = cache;
        }

        public async Task<MilestoneRuleResponse> HandleAsync(CreateMilestoneRuleCommand command)
        {
            var projectExists = await _projectRepository.ExistsAsync(command.RedmineProjectId);
            if (!projectExists)
                throw new Exception($"Project with Id {command.RedmineProjectId} does not exist.");

            var milestoneExists = await _projectMilestoneRepository
                .ExistsAsync(command.RedmineProjectId, command.MilestoneName);
            if (!milestoneExists)
                throw new Exception($"Milestone '{command.MilestoneName}' does not exist in project {command.RedmineProjectId}.");

            var allRules = (await _repository.GetAllAsync()).ToList();

            var activeRule = allRules.FirstOrDefault(r => r.IsActive
                && r.RedmineProjectId == command.RedmineProjectId
                && r.MilestoneName == command.MilestoneName);
            if (activeRule != null)
            {
                if (activeRule.BonusAmount == command.BonusAmount)
                    throw new Exception(
                        $"A milestone rule for project {command.RedmineProjectId} and milestone '{command.MilestoneName}' already exists and is active.");
                else
                    throw new Exception(
                        $"There is already an active milestone rule for project {command.RedmineProjectId} and milestone '{command.MilestoneName}' " +
                        $"with bonus {activeRule.BonusAmount:C}. Disable it before creating a different one.");
            }

            var inactiveRule = allRules.FirstOrDefault(r => !r.IsActive
                && r.RedmineProjectId == command.RedmineProjectId
                && r.MilestoneName == command.MilestoneName);
            if (inactiveRule != null)
            {
                if (inactiveRule.BonusAmount == command.BonusAmount)
                    throw new Exception($"A milestone rule for project {command.RedmineProjectId} and milestone {command.MilestoneName} with bonus {command.BonusAmount} already exists but is disabled. Enable it instead of creating a new one.");
            }

            var rule = new MilestoneRule(command.RedmineProjectId, command.MilestoneName, command.BonusAmount);
            await _repository.AddAsync(rule);
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveAsync("milestoneRules:all");

            return new MilestoneRuleResponse
            {
                Id = rule.Id,
                RedmineProjectId = rule.RedmineProjectId,
                MilestoneName = rule.MilestoneName,
                BonusAmount = rule.BonusAmount,
                IsActive = rule.IsActive
            };
        }
    }
}