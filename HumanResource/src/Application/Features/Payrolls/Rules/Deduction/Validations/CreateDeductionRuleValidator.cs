using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Deduction.Commands;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.Deduction.Validations
{
    public class CreateDeductionRuleValidator : AbstractValidator<CreateDeductionRuleCommand>
    {
        public CreateDeductionRuleValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");

            RuleFor(x => x.Percentage)
                .GreaterThan(0).WithMessage("Percentage must be greater than 0.")
                .LessThanOrEqualTo(1).WithMessage("Percentage cannot be greater than 1.");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("A DeductionType is required");
        }
    }
}