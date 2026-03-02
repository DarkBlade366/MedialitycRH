using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Deduction.Commands;
using Application.Features.Payrolls.Rules.Productivity.Commands;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.Productivity.Validations
{
    public class ChangeProductivityRuleStatusValidator : AbstractValidator<ChangeProductivityRuleStatusCommand>
    {
        public ChangeProductivityRuleStatusValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Productivity Id is required.");
        }
    }
}