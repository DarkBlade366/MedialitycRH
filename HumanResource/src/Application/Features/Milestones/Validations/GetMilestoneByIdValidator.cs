using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Milestones.Queries;
using FluentValidation;

namespace Application.Features.Milestones.Validations
{
    public class GetMilestoneByIdValidator: AbstractValidator<GetMilestoneByIdQuery>
    {
        public GetMilestoneByIdValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Milestone Id is required.");
        }
    }

}