using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.BaseSalary.Commands;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.BaseSalary.Handlers
{
    public class ChangeBaseSalaryRuleStatusHandler
    {
        private readonly IBaseSalaryRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
    
        public ChangeBaseSalaryRuleStatusHandler(
            IBaseSalaryRuleRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
    
        public async Task HandleAsync(ChangeBaseSalaryRuleStatusCommand command)
        {
            var rule = await _repository.GetByIdAsync(command.Id);

            var existingActive = (await _repository.GetAllAsync())
                .Any(r => r.IsActive);
        
            if (existingActive)
                throw new Exception("A base salary rule is already disabled; enable it.");
    
            if (rule is null)
                throw new Exception("Base salary rule not found.");
    
            if (command.IsActive)
                if (rule.IsActive)
                    throw new Exception("BaseSalary rule is already active.");
                else
                    rule.Activate();
            else
                if (!rule.IsActive)
                    throw new Exception("BaseSalary rule is already inactive.");
                else
                    rule.Deactivate();
    
            _repository.Update(rule);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}