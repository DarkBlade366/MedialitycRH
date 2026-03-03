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
    
        public CreateMilestoneRuleHandler(
            IMilestoneRuleRepository repository,
            IUnitOfWork unitOfWork,
            IProjectRepository projectRepository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _projectRepository = projectRepository;
        }
    
        public async Task<MilestoneRuleResponse> HandleAsync(CreateMilestoneRuleCommand command)
        {
            var projectExists = await _projectRepository.ExistsAsync(command.RedmineProjectId);
            
            if (!projectExists)
                throw new Exception($"Project with Id {command.RedmineProjectId} does not exist.");

            var rule = new MilestoneRule(
                command.RedmineProjectId,
                command.MilestoneName,
                command.BonusAmount);

            var existingActive = (await _repository.GetAllAsync())
                .Any(r => r.IsActive);
            var existingInactive = (await _repository.GetAllAsync())
                .Any(r => !r.IsActive);
        
            if (existingActive)
                throw new Exception("There is already an active milestone rule.");
            if (existingInactive)
                throw new Exception("A milestone rule is already disabled; enable it.");
    
            await _repository.AddAsync(rule);
            await _unitOfWork.SaveChangesAsync();
    
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