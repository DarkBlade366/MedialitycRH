using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Aguinaldo.DTOs;
using Application.Features.Payrolls.Rules.Aguinaldo.Queries;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Aguinaldo.Handlers
{
    public class GetAguinaldoRuleByIdHandler
    {
        private readonly IAguinaldoRuleRepository _repository;
    
        public GetAguinaldoRuleByIdHandler(IAguinaldoRuleRepository repository)
        {
            _repository = repository;
        }
    
        public async Task<AguinaldoRuleResponse?> HandleAsync(GetAguinaldoRuleByIdQuery query)
        {
            var rule = await _repository.GetByIdAsync(query.Id);
    
            if (rule is null)
                return null;
    
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