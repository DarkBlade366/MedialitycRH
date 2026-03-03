using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Projects.Queries;
using FluentValidation;

namespace Application.Features.Projects.Validations
{
    public class GetMilestoneParticipationByIdValidator : AbstractValidator<GetMilestoneParticipationByIdQuery>
    {
        public GetMilestoneParticipationByIdValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Participation Id is required.");
        }
    }
}