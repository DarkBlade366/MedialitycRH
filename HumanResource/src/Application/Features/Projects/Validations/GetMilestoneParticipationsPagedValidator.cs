using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Projects.Queries;
using FluentValidation;

namespace Application.Features.Projects.Validations
{
    public class GetMilestoneParticipationsPagedValidator : AbstractValidator<GetMilestoneParticipationsPagedQuery>
    {
        public GetMilestoneParticipationsPagedValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page must be >= 1");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("PageSize must be between 1 and 100");
        }
    }
}