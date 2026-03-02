using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Overtime.Commands;
using Application.Features.Payrolls.Rules.Overtime.DTOs;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Overtime.Handlers
{
    public class CreateOvertimeRuleHandler
    {
        private readonly IOvertimeRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
    
        public CreateOvertimeRuleHandler(
            IOvertimeRuleRepository repository, 
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<OvertimeRuleResponse> HandleAsync(CreateOvertimeRuleCommand command)
        {
            var rule = new OvertimeRule(
                command.StandardHoursPerPeriod,
                command.OvertimeMultiplier);

            var existingActive = (await _repository.GetAllAsync())
                .Any(r => r.IsActive);
            var existingInactive = (await _repository.GetAllAsync())
                .Any(r => !r.IsActive);
        
            if (existingActive)
                throw new Exception("There is already an active overtime rule.");
            if (existingInactive)
                throw new Exception("An overtime rule is already disabled; enable it.");

            await _repository.AddAsync(rule);
            await _unitOfWork.SaveChangesAsync();

            return new OvertimeRuleResponse
            {
                Id = rule.Id,
                StandardHoursPerPeriod = rule.StandardHoursPerPeriod,
                OvertimeMultiplier = rule.OvertimeMultiplier,
                IsActive = rule.IsActive
            };
        }
    }
}