using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Productivity.Commands;
using Domain.Features.Payrolls.Enums;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.Productivity.Validations
{
    public class CreateProductivityRuleValidator : AbstractValidator<CreateProductivityRuleCommand>
    {
        public CreateProductivityRuleValidator()
        {
            RuleFor(x => x.MinimumTarget)
                .GreaterThanOrEqualTo(0).WithMessage("MinimumTarget must be non-negative.");

            RuleFor(x => x.FullBonusTarget)
                .GreaterThan(x => x.MinimumTarget).WithMessage("FullBonusTarget must be greater than MinimumTarget.");

            RuleFor(x => x.BonusValue)
                .GreaterThan(0).WithMessage("BonusValue must be greater than 0.");

            RuleFor(x => x.BonusType)
                .Must(x => Enum.TryParse<BonusType>(x, true, out _))
                .WithMessage("Invalid BonusType.");

            RuleFor(x => x.MaxBonusCap)
                .GreaterThan(0)
                .When(x => x.MaxBonusCap.HasValue).WithMessage("MaxBonusCap must be greater than 0 if specified.");
        }
    }
}