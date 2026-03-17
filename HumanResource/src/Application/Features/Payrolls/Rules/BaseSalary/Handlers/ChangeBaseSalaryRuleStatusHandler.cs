using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.BaseSalary.Commands;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.BaseSalary.Handlers
{
    public class ChangeBaseSalaryRuleStatusHandler
    {
        private readonly IBaseSalaryRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public ChangeBaseSalaryRuleStatusHandler(
            IBaseSalaryRuleRepository repository,
            IUnitOfWork unitOfWork,
            ICacheService cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task HandleAsync(ChangeBaseSalaryRuleStatusCommand command)
        {
            var rule = await _repository.GetByIdAsync(command.Id);
            if (rule == null)
                throw new Exception("Base salary rule not found.");

            if (command.IsActive)
            {
                if (rule.IsActive)
                    throw new Exception("Base salary rule is already active.");

                var anotherActive = (await _repository.GetAllAsync())
                    .Any(r => r.Id != rule.Id && r.IsActive && r.Role == rule.Role);
                if (anotherActive)
                    throw new Exception($"Another active base salary rule for role '{rule.Role}' already exists; deactivate it first.");

                rule.Activate();
            }
            else
            {
                if (!rule.IsActive)
                    throw new Exception("Base salary rule is already inactive.");
                rule.Deactivate();
            }

            _repository.Update(rule);
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveAsync("baseSalaryRules:all");
            await _cache.RemoveAsync($"baseSalaryRule:{rule.Id}");
        }
    }
}