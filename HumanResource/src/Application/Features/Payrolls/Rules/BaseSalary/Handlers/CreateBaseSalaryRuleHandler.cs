using System;
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
        private readonly ICacheService _cache;

        public CreateBaseSalaryRuleHandler(
            IBaseSalaryRuleRepository repository,
            IUnitOfWork unitOfWork,
            ICacheService cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<BaseSalaryRuleResponse> HandleAsync(CreateBaseSalaryRuleCommand command)
        {
            var roleEnum = Enum.Parse<EmployeeRole>(command.Role, true);

            var allRules = (await _repository.GetAllAsync()).ToList();

            var existingActive = allRules.Any(x => x.Role == roleEnum && x.IsActive);
            var existingInactive = allRules.Any(x => x.Role == roleEnum && x.Amount == command.Amount && !x.IsActive);

            if (existingActive)
                throw new Exception("There is already an active base salary rule for this role.");
            if (existingInactive)
                throw new Exception($"A base salary rule for role {command.Role} with amount ${command.Amount} is already disabled; enable it.");

            var rule = new BaseSalaryRule(roleEnum, command.Amount);

            await _repository.AddAsync(rule);
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveAsync("baseSalaryRules:all");

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