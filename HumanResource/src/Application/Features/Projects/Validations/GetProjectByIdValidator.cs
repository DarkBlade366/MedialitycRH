using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Application.Features.Projects.Queries;

namespace Application.Features.Projects.Validations
{
    public class GetProjectByIdValidator : AbstractValidator<GetProjectByIdQuery>
    {
        public GetProjectByIdValidator()
        {
            RuleFor(x => x.RedmineProjectId)
                .GreaterThan(0)
                .WithMessage("RedmineProjectId must be greater than 0.");
        }
    }
}