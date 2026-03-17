using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Overtime.Commands;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Overtime.Handlers
{
    public class ChangeOvertimeRuleStatusHandler
    {
        private readonly IOvertimeRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public ChangeOvertimeRuleStatusHandler(
            IOvertimeRuleRepository repository,
            IUnitOfWork unitOfWork,
            ICacheService cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task HandleAsync(ChangeOvertimeRuleStatusCommand command)
        {
            var rule = await _repository.GetByIdAsync(command.Id);
            if (rule == null)
                throw new Exception("Overtime rule not found.");

            if (command.IsActive)
            {
                if (rule.IsActive)
                    throw new Exception("Overtime rule is already active.");

                var anotherActive = (await _repository.GetAllAsync()).Any(r => r.Id != rule.Id && r.IsActive);
                if (anotherActive)
                    throw new Exception("There is already an active overtime rule; deactivate it before activating this one.");

                rule.Activate();
            }
            else
            {
                if (!rule.IsActive)
                    throw new Exception("Overtime rule is already inactive.");
                rule.Deactivate();
            }

            _repository.Update(rule);
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveAsync("overtimeRules:all");
            await _cache.RemoveAsync($"overtimeRule:{rule.Id}");
        }
    }
}