using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Milestones.Queries;
using FluentValidation;

namespace Application.Features.Milestones.Validations
{
    public class GetMilestonesPagedValidator: AbstractValidator<GetMilestonesPagedQuery>
    {
        public GetMilestonesPagedValidator()
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