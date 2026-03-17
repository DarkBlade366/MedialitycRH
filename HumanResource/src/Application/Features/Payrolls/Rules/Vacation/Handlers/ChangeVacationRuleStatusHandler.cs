using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Vacation.Commands;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Vacation.Handlers
{
    public class ChangeVacationRuleStatusHandler
    {
        private readonly IVacationRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public ChangeVacationRuleStatusHandler(
            IVacationRuleRepository repository,
            IUnitOfWork unitOfWork,
            ICacheService cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task HandleAsync(ChangeVacationRuleStatusCommand command)
        {
            var rule = await _repository.GetByIdAsync(command.Id);
            if (rule == null)
                throw new Exception("Vacation rule not found.");

            if (command.IsActive)
            {
                if (rule.IsActive)
                    throw new Exception("Vacation rule is already active.");

                var anotherActive = (await _repository.GetAllAsync()).Any(r => r.Id != rule.Id && r.IsActive);
                if (anotherActive)
                    throw new Exception("There is already an active vacation rule; deactivate it before activating this one.");

                rule.Activate();
            }
            else
            {
                if (!rule.IsActive)
                    throw new Exception("Vacation rule is already inactive.");

                rule.Deactivate();
            }

            _repository.Update(rule);
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveAsync("vacationRules:all");
            await _cache.RemoveAsync($"vacationRule:{rule.Id}");
        }
    }
}