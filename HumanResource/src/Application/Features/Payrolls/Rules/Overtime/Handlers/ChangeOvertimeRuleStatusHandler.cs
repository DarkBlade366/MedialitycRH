using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Overtime.Commands;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Overtime.Handlers
{
    public class ChangeOvertimeRuleStatusHandler
    {
        private readonly IOvertimeRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
    
        public ChangeOvertimeRuleStatusHandler(
            IOvertimeRuleRepository repository, 
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(ChangeOvertimeRuleStatusCommand command)
        {
            var rule = await _repository.GetByIdAsync(command.Id);
    
            if (rule is null)
                throw new Exception("Overtime rule not found.");

            var existingActive = (await _repository.GetAllAsync())
                .Any(r => r.IsActive);
        
            if (existingActive)
                throw new Exception("An overtime rule is already disabled; enable it.");
    
            if (command.IsActive)
                if (rule.IsActive)
                    throw new Exception("Overtime rule is already active.");
                else
                    rule.Activate();
            else
                if (!rule.IsActive)
                    throw new Exception("Overtime rule is already inactive.");
                else
                    rule.Deactivate();
    
            _repository.Update(rule);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}