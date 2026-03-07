using FluentValidation;
using Application.Features.Payrolls.Payments.Project.Queries;

namespace Application.Features.Payrolls.Payments.Project.Validations
{
    public class GetProjectPaymentsPagedValidator : AbstractValidator<GetProjectPaymentsPagedQuery>
    {
        public GetProjectPaymentsPagedValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Page must be greater than 0.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("PageSize must be between 1 and 100.");

            RuleFor(x => x.RedmineProjectId)
                .GreaterThan(0)
                .When(x => x.RedmineProjectId.HasValue)
                .WithMessage("RedmineProjectId must be greater than 0 when specified.");

            RuleFor(x => x.From)
                .LessThan(x => x.To)
                .When(x => x.From.HasValue && x.To.HasValue)
                .WithMessage("From date must be before To date.");
        }
    }
}
