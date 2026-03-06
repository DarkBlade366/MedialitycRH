using Application.Features.Payrolls.Rules.ActivityProductivityWeight.Queries;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.ActivityProductivityWeight.Validations
{
    public class GetActivityProductivityWeightByIdValidator : AbstractValidator<GetActivityProductivityWeightByIdQuery>
    {
        public GetActivityProductivityWeightByIdValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");
        }
    }
}
