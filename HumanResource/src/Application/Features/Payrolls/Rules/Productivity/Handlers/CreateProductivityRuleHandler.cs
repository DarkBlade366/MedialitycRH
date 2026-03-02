using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Productivity.Commands;
using Application.Features.Payrolls.Rules.Productivity.DTOs;
using Domain.Features.Payrolls.Enums;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Productivity.Handlers
{
    public class CreateProductivityRuleHandler
    {
        private readonly IProductivityRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
    
        public CreateProductivityRuleHandler(
            IProductivityRuleRepository repository, 
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductivityRuleResponse> HandleAsync(CreateProductivityRuleCommand command)
        {
            var bonusType = Enum.Parse<BonusType>(command.BonusType, true);
    
            var rule = new ProductivityRule(
                command.MinimumTarget,
                command.FullBonusTarget,
                command.BonusValue,
                bonusType,
                command.MaxBonusCap);
    
            var existingActive = (await _repository.GetAllAsync())
                .Any(r => r.IsActive);
            var existingInactive = (await _repository.GetAllAsync())
                .Any(r => !r.IsActive);
        
            if (existingActive)
                throw new Exception("There is already an active productivity rule.");
            if (existingInactive)
                throw new Exception("A productivity rule is already disabled; enable it.");
            
            await _repository.AddAsync(rule);
            await _unitOfWork.SaveChangesAsync();
    
            return new ProductivityRuleResponse
            {
                Id = rule.Id,
                MinimumTarget = rule.MinimumTarget,
                FullBonusTarget = rule.FullBonusTarget,
                BonusValue = rule.BonusValue,
                BonusType = rule.BonusType.ToString(),
                MaxBonusCap = rule.MaxBonusCap,
                IsActive = rule.IsActive
            };
        }
    }
}