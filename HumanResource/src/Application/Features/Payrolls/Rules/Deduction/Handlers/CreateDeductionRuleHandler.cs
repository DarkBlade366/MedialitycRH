using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Deduction.Commands;
using Application.Features.Payrolls.Rules.Deduction.DTOs;
using Domain.Features.Payrolls.Enums;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Deduction.Handlers
{
    public class CreateDeductionRuleHandler
    {
        private readonly IDeductionRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public CreateDeductionRuleHandler(
            IDeductionRuleRepository repository,
            IUnitOfWork unitOfWork,
            ICacheService cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<DeductionRuleResponse> HandleAsync(CreateDeductionRuleCommand command)
        {
            var typeEnum = Enum.Parse<DeductionType>(command.Type, true);

            var allRules = (await _repository.GetAllAsync()).ToList();

            var activeSame = allRules.Any(r =>
                r.IsActive &&
                r.Type == typeEnum &&
                r.Description == command.Description);
            if (activeSame)
                throw new Exception(
                    $"An active deduction rule with type '{command.Type}' " +
                    $"and description '{command.Description}' already exists.");

            var identical = allRules.FirstOrDefault(r =>
                r.Type == typeEnum &&
                r.Description == command.Description &&
                r.Percentage == command.Percentage);

            if (identical != null)
            {
                if (identical.IsActive)
                    throw new Exception(
                        $"A deduction rule of type '{command.Type}', description '{command.Description}' " +
                        $"and percentage {command.Percentage} already exists and is active.");
                else
                    throw new Exception(
                        $"A deduction rule of type '{command.Type}', description '{command.Description}' " +
                        $"and percentage {command.Percentage} already exists but is disabled. Enable it instead of creating a new one.");
            }

            var rule = new DeductionRule(command.Percentage, command.Description, typeEnum);

            await _repository.AddAsync(rule);
            await _unitOfWork.SaveChangesAsync();

            await _cache.RemoveAsync("deductionRules:all");

            return new DeductionRuleResponse
            {
                Id = rule.Id,
                Description = rule.Description,
                Percentage = rule.Percentage,
                Type = rule.Type.ToString(),
                IsActive = rule.IsActive
            };
        }
    }
}