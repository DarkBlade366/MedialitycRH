using FluentValidation;
using Application.Features.Payrolls.Rules.Project.Commands;

namespace Application.Features.Payrolls.Rules.Project.Validations
{
    public class CreateProjectRuleValidator : AbstractValidator<CreateProjectRuleCommand>
    {
        public CreateProjectRuleValidator()
        {
            RuleFor(x => x.RedmineProjectId)
                .GreaterThan(0)
                .WithMessage("RedmineProjectId must be greater than 0.");

            RuleFor(x => x.BonusAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("BonusAmount cannot be negative.");
        }
    }
}
