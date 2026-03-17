using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Overtime.Commands;
using Application.Features.Payrolls.Rules.Overtime.DTOs;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Overtime.Handlers
{
    public class CreateOvertimeRuleHandler
    {
        private readonly IOvertimeRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public CreateOvertimeRuleHandler(
            IOvertimeRuleRepository repository,
            IUnitOfWork unitOfWork,
            ICacheService cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<OvertimeRuleResponse> HandleAsync(CreateOvertimeRuleCommand command)
        {
            var allRules = (await _repository.GetAllAsync()).ToList();

            if (allRules.Any(r => r.IsActive))
                throw new Exception("There is already an active overtime rule; only one can be active at a time.");

            var identical = allRules.FirstOrDefault(r =>
                r.StandardHoursPerPeriod == command.StandardHoursPerPeriod &&
                r.OvertimeMultiplier == command.OvertimeMultiplier &&
                r.HourlyRate == command.HourlyRate);

            if (identical != null)
            {
                if (identical.IsActive)
                    throw new Exception($"An overtime rule with {command.StandardHoursPerPeriod} standard hours, multiplier {command.OvertimeMultiplier} and rate {command.HourlyRate:C} already exists and is active.");
                else
                    throw new Exception($"An overtime rule with {command.StandardHoursPerPeriod} standard hours, multiplier {command.OvertimeMultiplier} and rate {command.HourlyRate:C} already exists but is disabled. Enable it instead of creating a new one.");
            }

            var rule = new OvertimeRule(command.StandardHoursPerPeriod, command.OvertimeMultiplier, command.HourlyRate);
            await _repository.AddAsync(rule);
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveAsync("overtimeRules:all");

            return new OvertimeRuleResponse
            {
                Id = rule.Id,
                StandardHoursPerPeriod = rule.StandardHoursPerPeriod,
                OvertimeMultiplier = rule.OvertimeMultiplier,
                HourlyRate = rule.HourlyRate,
                IsActive = rule.IsActive
            };
        }
    }
}