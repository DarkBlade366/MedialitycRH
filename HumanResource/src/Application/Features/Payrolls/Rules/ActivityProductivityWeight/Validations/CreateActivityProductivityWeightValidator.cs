using Application.Features.Payrolls.Rules.ActivityProductivityWeight.Commands;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.ActivityProductivityWeight.Validations
{
    public class CreateActivityProductivityWeightValidator : AbstractValidator<CreateActivityProductivityWeightCommand>
    {
        public CreateActivityProductivityWeightValidator()
        {
            RuleFor(x => x.RedmineActivityId)
                .GreaterThan(0).WithMessage("RedmineActivityId must be greater than 0.");

            RuleFor(x => x.ActivityName)
                .NotEmpty().WithMessage("ActivityName is required.")
                .MaximumLength(100).WithMessage("ActivityName cannot exceed 100 characters.");

            RuleFor(x => x.Weight)
                .InclusiveBetween(0m, 1m).WithMessage("Weight must be between 0 and 1.");
        }
    }
}
