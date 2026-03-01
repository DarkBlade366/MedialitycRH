using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Milestones.Commands;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.Milestones.Validations
{
    public class CreateMilestoneRuleValidator : AbstractValidator<CreateMilestoneRuleCommand>
    {
        public CreateMilestoneRuleValidator()
        {
            RuleFor(x => x.RedmineProjectId)
                .GreaterThan(0)
                .WithMessage("RedmineProjectId must be greater than 0.");

            RuleFor(x => x.MilestoneName)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("MilestoneName is required and must be at most 200 characters long.");

            RuleFor(x => x.BonusAmount)
                .GreaterThan(0)
                .WithMessage("BonusAmount must be greater than 0.");
        }
    }
}