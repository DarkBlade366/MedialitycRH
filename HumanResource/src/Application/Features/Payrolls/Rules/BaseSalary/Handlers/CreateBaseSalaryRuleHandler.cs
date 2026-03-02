using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.BaseSalary.Commands;
using Application.Features.Payrolls.Rules.BaseSalary.DTOs;
using Domain.Features.Employees.Enums;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.BaseSalary.Handlers
{
    public class CreateBaseSalaryRuleHandler
    {
        private readonly IBaseSalaryRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
    
        public CreateBaseSalaryRuleHandler(
            IBaseSalaryRuleRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
    
        public async Task<BaseSalaryRuleResponse> HandleAsync(CreateBaseSalaryRuleCommand command)
        {
            var roleEnum = Enum.Parse<EmployeeRole>(command.Role, true);

            var rule = new BaseSalaryRule(roleEnum, command.Amount);
            
            var existingActive = (await _repository.GetAllAsync())
                .Any(x => x.Role == roleEnum && x.IsActive);
            var existingInactive = (await _repository.GetAllAsync())
                .Any(x => x.Role == roleEnum && !x.IsActive);
    
            if (existingActive)
                throw new Exception("There is already an active base salary rule for this role.");
            if (existingActive)
                throw new Exception("A base salary rule for this role is already disabled; enable it.");
    
            await _repository.AddAsync(rule);
            await _unitOfWork.SaveChangesAsync();
    
            return new BaseSalaryRuleResponse
            {
                Id = rule.Id,
                Role = rule.Role.ToString(),
                Amount = rule.Amount,
                IsActive = rule.IsActive
            };
        }
    }
}