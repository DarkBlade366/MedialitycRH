using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Application.Features.Analytics.Queries;

namespace Application.Features.Analytics.Validations
{
    public class GetProjectCostsValidator : AbstractValidator<GetProjectCostsQuery>
    {
        public GetProjectCostsValidator()
        {
            RuleFor(x => x.PeriodStart)
                .NotEmpty().WithMessage("PeriodStart is required.");

            RuleFor(x => x.PeriodEnd)
                .NotEmpty().WithMessage("PeriodEnd is required.")
                .GreaterThanOrEqualTo(x => x.PeriodStart)
                    .WithMessage("PeriodEnd must be after or equal to PeriodStart.");

            RuleFor(x => x.ProjectId)
                .GreaterThan(0).When(x => x.ProjectId.HasValue)
                    .WithMessage("ProjectId must be greater than 0.");
        }
    }
}