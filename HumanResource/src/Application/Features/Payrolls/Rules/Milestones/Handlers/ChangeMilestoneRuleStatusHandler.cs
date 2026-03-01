using System;
using System.Collections.Generic;
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
    
        public ChangeMilestoneRuleStatusHandler(
            IMilestoneRuleRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
    
        public async Task HandleAsync(ChangeMilestoneRuleStatusCommand command)
        {
            var rule = await _repository.GetByIdAsync(command.Id);
    
            if (rule is null)
                throw new Exception("Milestone rule not found.");
    
            if (command.IsActive)
                if (rule.IsActive)
                    throw new Exception("Milestone rule is already active.");
                else
                    rule.Activate();
            else
                if (!rule.IsActive)
                    throw new Exception("Milestone rule is already inactive.");
                else
                    rule.Deactivate();
    
            _repository.Update(rule);
    
            await _unitOfWork.SaveChangesAsync();
        }
    }
}