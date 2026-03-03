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
            var allRules = (await _repository.GetAllAsync()).ToList();

            var active = allRules.FirstOrDefault(r => r.IsActive);
            if (active != null)
                throw new Exception(
                    $"There is already an active vacation rule with accrual rate {active.AccrualRatePerMonth}.");

            var identical = allRules.FirstOrDefault(r =>
                r.AccrualRatePerMonth == command.AccrualRatePerMonth);

            if (identical != null)
            {
                if (identical.IsActive)
                    throw new Exception(
                        $"A vacation rule with accrual rate {command.AccrualRatePerMonth} is already active.");
                else
                    throw new Exception(
                        $"A vacation rule with accrual rate {command.AccrualRatePerMonth} already exists but is disabled. " +
                        "Enable it instead of creating a new one.");
            }

            var rule = new VacationRule(command.AccrualRatePerMonth);

            await _repository.AddAsync(rule);
            await _unitOfWork.SaveChangesAsync();

            return new VacationRuleResponse
            {
                Id = rule.Id,
                AccrualRatePerMonth = rule.AccrualRatePerMonth,
                IsActive = rule.IsActive
            };
        }
    }
}