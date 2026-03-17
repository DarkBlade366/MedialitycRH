using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Productivity.Commands;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Productivity.Handlers
{
    public class ChangeProductivityRuleStatusHandler
    {
        private readonly IProductivityRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public ChangeProductivityRuleStatusHandler(
            IProductivityRuleRepository repository,
            IUnitOfWork unitOfWork,
            ICacheService cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task HandleAsync(ChangeProductivityRuleStatusCommand command)
        {
            var rule = await _repository.GetByIdAsync(command.Id);
            if (rule == null)
                throw new Exception("Productivity rule not found.");

            if (command.IsActive)
            {
                if (rule.IsActive)
                    throw new Exception("Productivity rule is already active.");

                var anotherActive = (await _repository.GetAllAsync()).Any(r => r.Id != rule.Id && r.IsActive);
                if (anotherActive)
                    throw new Exception("There is already an active productivity rule; deactivate it before activating this one.");

                rule.Activate();
            }
            else
            {
                if (!rule.IsActive)
                    throw new Exception("Productivity rule is already inactive.");
                rule.Deactivate();
            }

            _repository.Update(rule);
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveAsync("productivityRules:all");
            await _cache.RemoveAsync($"productivityRule:{rule.Id}");
        }
    }
}