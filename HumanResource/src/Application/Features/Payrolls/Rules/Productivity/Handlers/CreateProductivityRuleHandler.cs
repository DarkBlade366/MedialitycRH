using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Productivity.Commands;
using Application.Features.Payrolls.Rules.Productivity.DTOs;
using Domain.Features.Payrolls.Enums;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Productivity.Handlers
{
    public class CreateProductivityRuleHandler
    {
        private readonly IProductivityRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public CreateProductivityRuleHandler(
            IProductivityRuleRepository repository,
            IUnitOfWork unitOfWork,
            ICacheService cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<ProductivityRuleResponse> HandleAsync(CreateProductivityRuleCommand command)
        {
            var bonusType = Enum.Parse<BonusType>(command.BonusType, true);
            var all = (await _repository.GetAllAsync()).ToList();

            if (all.Any(r => r.IsActive))
                throw new Exception("There is already an active productivity rule; deactivate it first.");

            var identical = all.FirstOrDefault(r =>
                r.MinimumTarget == command.MinimumTarget &&
                r.FullBonusTarget == command.FullBonusTarget &&
                r.BonusValue == command.BonusValue &&
                r.BonusType == bonusType &&
                r.MaxBonusCap == command.MaxBonusCap);

            if (identical != null)
            {
                if (identical.IsActive)
                    throw new Exception($"A productivity rule (min {command.MinimumTarget}, full {command.FullBonusTarget}, bonus {command.BonusValue} {command.BonusType}, cap {command.MaxBonusCap}) already exists and is active.");
                else
                    throw new Exception($"A productivity rule (min {command.MinimumTarget}, full {command.FullBonusTarget}, bonus {command.BonusValue} {command.BonusType}, cap {command.MaxBonusCap}) already exists but is disabled. Enable it instead of creating a new one.");
            }

            var rule = new ProductivityRule(command.MinimumTarget, command.FullBonusTarget, command.BonusValue, bonusType, command.MaxBonusCap);
            
            await _repository.AddAsync(rule);
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveAsync("productivityRules:all");

            return new ProductivityRuleResponse
            {
                Id = rule.Id,
                MinimumTarget = rule.MinimumTarget,
                FullBonusTarget = rule.FullBonusTarget,
                BonusValue = rule.BonusValue,
                BonusType = rule.BonusType.ToString(),
                MaxBonusCap = rule.MaxBonusCap,
                IsActive = rule.IsActive
            };
        }
    }
}