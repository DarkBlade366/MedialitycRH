using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Deduction.Commands;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Deduction.Handlers
{
    public class ChangeDeductionRuleStatusHandler
    {
        private readonly IDeductionRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
    
        public ChangeDeductionRuleStatusHandler(
            IDeductionRuleRepository repository, 
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(ChangeDeductionRuleStatusCommand command)
        {
            var rule = await _repository.GetByIdAsync(command.Id);
    
            if (rule is null)
                throw new Exception("Deduction rule not found.");
    
            if (command.IsActive)
                if (rule.IsActive)
                    throw new Exception("Deduction rule is already active.");
                else
                    rule.Activate();
            else
                if (!rule.IsActive)
                    throw new Exception("Deduction rule is already inactive.");
                else
                    rule.Deactivate();
    
            _repository.Update(rule);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}