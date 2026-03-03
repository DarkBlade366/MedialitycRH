using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Aguinaldo.Commands;
using Application.Features.Payrolls.Rules.Aguinaldo.DTOs;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Aguinaldo.Handlers
{
    public class CreateAguinaldoRuleHandler
    {
        private readonly IAguinaldoRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateAguinaldoRuleHandler(
            IAguinaldoRuleRepository repository, 
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<AguinaldoRuleResponse> HandleAsync(CreateAguinaldoRuleCommand command)
        {
            var allRules = (await _repository.GetAllAsync()).ToList();

            var existingActive = allRules.FirstOrDefault(r => r.IsActive);
            if (existingActive != null)
                throw new Exception("There is already an active aguinaldo rule.");

            var existingIdentical = allRules.FirstOrDefault(r =>
                r.MonthlyAccrualPercentage == command.MonthlyAccrualPercentage &&
                r.PayMonth == command.PayMonth);

            if (existingIdentical != null)
            {
                if (existingIdentical.IsActive)
                    throw new Exception("There is already an active aguinaldo rule.");
                else
                    throw new Exception($"An aguinaldo rule with {command.MonthlyAccrualPercentage}% monthly accrual for month {command.PayMonth} already exists but is disabled. Enable it instead of creating a new one.");
            }

            var rule = new AguinaldoRule(
                command.MonthlyAccrualPercentage,
                command.PayMonth);
            
            await _repository.AddAsync(rule);
            await _unitOfWork.SaveChangesAsync();

            return new AguinaldoRuleResponse
            {
                Id = rule.Id,
                MonthlyAccrualPercentage = rule.MonthlyAccrualPercentage,
                PayMonth = rule.PayMonth,
                IsActive = rule.IsActive
            };
        }
    }
}