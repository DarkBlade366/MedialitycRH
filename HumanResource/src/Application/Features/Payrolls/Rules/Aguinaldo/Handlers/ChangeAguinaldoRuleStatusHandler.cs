using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Aguinaldo.Commands;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Aguinaldo.Handlers
{
    public class ChangeAguinaldoRuleStatusHandler
    {
        private readonly IAguinaldoRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public ChangeAguinaldoRuleStatusHandler(
            IAguinaldoRuleRepository repository,
            IUnitOfWork unitOfWork,
            ICacheService cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task HandleAsync(ChangeAguinaldoRuleStatusCommand command)
        {
            var rule = await _repository.GetByIdAsync(command.Id);
            if (rule == null)
                throw new Exception("Aguinaldo rule not found.");

            if (command.IsActive)
            {
                if (rule.IsActive)
                    throw new Exception("Aguinaldo rule is already active.");

                var anotherActive = (await _repository.GetAllAsync()).Any(r => r.Id != rule.Id && r.IsActive);
                if (anotherActive)
                    throw new Exception("There is already an active aguinaldo rule; deactivate it first.");

                rule.Activate();
            }
            else
            {
                if (!rule.IsActive)
                    throw new Exception("Aguinaldo rule is already inactive.");

                rule.Deactivate();
            }

            _repository.Update(rule);
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveAsync("aguinaldoRules:all");
            await _cache.RemoveAsync($"aguinaldoRule:{rule.Id}");
        }
    }
}