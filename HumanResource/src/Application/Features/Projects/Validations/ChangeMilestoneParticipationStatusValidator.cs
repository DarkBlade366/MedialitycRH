using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Projects.Commands;
using FluentValidation;

namespace Application.Features.Projects.Validations
{
    public class ChangeMilestoneParticipationStatusValidator : AbstractValidator<ChangeMilestoneParticipationStatusCommand>
    {
        public ChangeMilestoneParticipationStatusValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Participation Id is required.");
        }
    }
}