using Application.Features.Payrolls.Rules.ActivityProductivityWeight.Queries;
using FluentValidation;

namespace Application.Features.Payrolls.Rules.ActivityProductivityWeight.Validations
{
    public class GetActivityProductivityWeightsPagedValidator : AbstractValidator<GetActivityProductivityWeightsPagedQuery>
    {
        public GetActivityProductivityWeightsPagedValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("Page must be greater than 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("PageSize must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("PageSize cannot exceed 100.");
        }
    }
}
