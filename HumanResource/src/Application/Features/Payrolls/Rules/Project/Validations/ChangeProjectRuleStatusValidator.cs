using FluentValidation;
using Application.Features.Payrolls.Rules.Project.Commands;

namespace Application.Features.Payrolls.Rules.Project.Validations
{
    public class ChangeProjectRuleStatusValidator : AbstractValidator<ChangeProjectRuleStatusCommand>
    {
        public ChangeProjectRuleStatusValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.");
        }
    }
}
