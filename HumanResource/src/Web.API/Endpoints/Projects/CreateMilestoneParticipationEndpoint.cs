using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using Application.Features.Projects.Commands;
using Application.Features.Projects.DTOs;
using Application.Features.Projects.Handlers;
using Application.Features.Projects.Queries;
using Application.Features.Projects.Validations;

namespace Web.API.Endpoints.Projects
{
    public class CreateMilestoneParticipationEndpoint : Endpoint<CreateMilestoneParticipationCommand, MilestoneParticipationResponse>
    {
        private readonly CreateMilestoneParticipationHandler _handler;

        public CreateMilestoneParticipationEndpoint(CreateMilestoneParticipationHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Post("/milestone-participations");
            Roles("Administrator");
            Validator<CreateMilestoneParticipationValidator>();
            Summary(s =>
            {
                s.Summary = "Creates a milestone participation";
                s.Description = "Creates a new milestone participation for a milestone and employee.";
                s.ExampleRequest = new CreateMilestoneParticipationCommand
                {
                    ProjectMilestoneId = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    EmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000000")
                };
            });
        }

        public override async Task HandleAsync(CreateMilestoneParticipationCommand req, CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result);
        }
    }
}