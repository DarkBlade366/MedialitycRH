using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Deduction.Commands;
using Application.Features.Payrolls.Rules.Deduction.DTOs;
using Domain.Features.Payrolls.Enums;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Deduction.Handlers
{
    public class CreateDeductionRuleHandler
    {
        private readonly IDeductionRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
    
        public CreateDeductionRuleHandler(
            IDeductionRuleRepository repository, 
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<DeductionRuleResponse> HandleAsync(CreateDeductionRuleCommand command)
        {
            var roleEnum = Enum.Parse<DeductionType>(command.Type, true);

            var rule = new DeductionRule(
                command.Percentage,
                command.Description,
                roleEnum);

            var existingActive = (await _repository.GetAllAsync())
                .Any(r => r.IsActive);
            var existingInactive = (await _repository.GetAllAsync())
                .Any(r => !r.IsActive);
        
            if (existingActive)
                throw new Exception("There is already an active deduction rule.");
            if (existingInactive)
                throw new Exception("A deduction rule is already disabled; enable it.");
    
            await _repository.AddAsync(rule);
            await _unitOfWork.SaveChangesAsync();
    
            return new DeductionRuleResponse
            {
                Id = rule.Id,
                Description = rule.Description,
                Percentage = rule.Percentage,
                Type = rule.Type.ToString(),
                IsActive = rule.IsActive
            };
        }
    }
}