using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Application.Features.Projects.Queries;

namespace Application.Features.Projects.Validations
{
    public class GetProjectsPagedValidator : AbstractValidator<GetProjectsPagedQuery>
    {
        public GetProjectsPagedValidator()
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