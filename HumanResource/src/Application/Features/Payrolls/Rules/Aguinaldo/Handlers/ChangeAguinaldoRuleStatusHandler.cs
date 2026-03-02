using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Aguinaldo.Commands;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Aguinaldo.Handlers
{
    public class ChangeAguinaldoRuleStatusHandler
    {
        private readonly IAguinaldoRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
    
        public ChangeAguinaldoRuleStatusHandler(
            IAguinaldoRuleRepository repository, 
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
    
        public async Task HandleAsync(ChangeAguinaldoRuleStatusCommand command)
        {
            var rule = await _repository.GetByIdAsync(command.Id);

            var existingActive = (await _repository.GetAllAsync())
                .Any(r => r.IsActive);
        
            if (existingActive)
                throw new Exception("An aguinaldo rule is already disabled; enable it.");
    
            if (rule is null)
                throw new Exception("Aguinaldo rule not found.");
    
            if (command.IsActive)
                if (rule.IsActive)
                    throw new Exception("Aguinaldo rule is already active.");
                else
                    rule.Activate();
            else
                if (!rule.IsActive)
                    throw new Exception("Aguinaldo rule is already inactive.");
                else
                    rule.Deactivate();
    
            _repository.Update(rule);
            await _unitOfWork.SaveChangesAsync();
        } 
    }
}