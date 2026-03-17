using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Deduction.Commands;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Deduction.Handlers
{
    public class ChangeDeductionRuleStatusHandler
    {
        private readonly IDeductionRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public ChangeDeductionRuleStatusHandler(
            IDeductionRuleRepository repository,
            IUnitOfWork unitOfWork,
            ICacheService cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task HandleAsync(ChangeDeductionRuleStatusCommand command)
        {
            var rule = await _repository.GetByIdAsync(command.Id);
            if (rule == null)
                throw new Exception("Deduction rule not found.");

            if (command.IsActive)
            {
                if (rule.IsActive)
                    throw new Exception("Deduction rule is already active.");

                var anyOtherActive = (await _repository.GetAllAsync())
                    .Any(r => r.Id != rule.Id
                                && r.IsActive
                                && r.Type == rule.Type
                                && r.Description == rule.Description);
                if (anyOtherActive)
                    throw new Exception(
                        $"Another active deduction rule with type '{rule.Type}' and description '{rule.Description}' already exists; deactivate it first.");

                rule.Activate();
            }
            else
            {
                if (!rule.IsActive)
                    throw new Exception("Deduction rule is already inactive.");
                rule.Deactivate();
            }

            _repository.Update(rule);
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveAsync("deductionRules:all");
            await _cache.RemoveAsync($"deductionRule:{rule.Id}");
        }
    }
}