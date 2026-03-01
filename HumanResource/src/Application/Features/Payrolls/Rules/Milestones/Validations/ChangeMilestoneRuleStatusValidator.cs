using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Milestones.Commands;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.Milestones.Validations
{
    public class ChangeMilestoneRuleStatusValidator : AbstractValidator<ChangeMilestoneRuleStatusCommand>
    {
        public ChangeMilestoneRuleStatusValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Employee Id is required.");
        }
    }
}