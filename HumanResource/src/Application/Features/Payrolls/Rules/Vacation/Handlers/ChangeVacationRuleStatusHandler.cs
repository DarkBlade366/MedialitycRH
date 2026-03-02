using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Vacation.Commands;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Vacation.Handlers
{
    public class ChangeVacationRuleStatusHandler
    {
        private readonly IVacationRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
    
        public ChangeVacationRuleStatusHandler(
            IVacationRuleRepository repository, 
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(ChangeVacationRuleStatusCommand command)
        {
            var rule = await _repository.GetByIdAsync(command.Id);
    
            if (rule == null)
                throw new Exception("Vacation rule not found.");
    
            if (command.IsActive)
                if (rule.IsActive)
                    throw new Exception("Vacation rule is already active.");
                else
                rule.Activate();
            else
                if (!rule.IsActive)
                    throw new Exception("Vacation rule is already inactive.");
                else
                    rule.Deactivate();
    
            _repository.Update(rule);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}