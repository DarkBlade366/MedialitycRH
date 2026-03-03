using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Projects.Commands;
using FluentValidation;

namespace Application.Features.Projects.Validations
{
    public class CreateMilestoneParticipationValidator : AbstractValidator<CreateMilestoneParticipationCommand>
    {
        public CreateMilestoneParticipationValidator()
        {
            RuleFor(x => x.ProjectMilestoneId)
                .NotEmpty().WithMessage("Project Milestone Id is required.");
            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithMessage("Employee Id is required.");
        }
    }
}