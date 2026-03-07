using FluentValidation;
using Application.Features.Payrolls.Rules.Project.Queries;

namespace Application.Features.Payrolls.Rules.Project.Validations
{
    public class GetProjectRulesPagedValidator : AbstractValidator<GetProjectRulesPagedQuery>
    {
        public GetProjectRulesPagedValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Page must be greater than 0.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("PageSize must be between 1 and 100.");
        }
    }
}
