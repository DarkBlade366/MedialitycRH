using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Vacation.Commands;
using Application.Features.Payrolls.Rules.Vacation.DTOs;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Vacation.Handlers
{
    public class CreateVacationRuleHandler
    {
        private readonly IVacationRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
    
        public CreateVacationRuleHandler(
            IVacationRuleRepository repository, 
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<VacationRuleResponse> HandleAsync(CreateVacationRuleCommand command)
        {
            var existingActive = (await _repository.GetAllAsync())
                .Any(r => r.IsActive);
            var existingInactive = (await _repository.GetAllAsync())
                .Any(r => !r.IsActive);
        
            if (existingActive)
                throw new Exception("There is already an active vacation rule.");
            if (existingInactive)
                throw new Exception("A vacation rule is already disabled; enable it.");

            var rule = new VacationRule(
                command.AccrualRatePerMonth,
                command.PayVacationOnUse);

            await _repository.AddAsync(rule);
            await _unitOfWork.SaveChangesAsync();

            return new VacationRuleResponse
            {
                Id = rule.Id,
                AccrualRatePerMonth = rule.AccrualRatePerMonth,
                PayVacationOnUse = rule.PayVacationOnUse,
                IsActive = rule.IsActive
            };
        }

        // var existingActive = (await _repository.GetAllAsync())
        //         .Any(r => r.IsActive);
        
        //     if (existingActive)
        //         throw new Exception("There is already an active vacation rule.");
    }
}