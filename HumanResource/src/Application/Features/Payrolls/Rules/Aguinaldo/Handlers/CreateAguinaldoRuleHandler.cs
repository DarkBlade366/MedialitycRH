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