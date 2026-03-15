using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using Application.Features.Projects.Commands;
using Application.Features.Projects.Handlers;
using Application.Features.Projects.Validations;

namespace Web.API.Endpoints.Projects
{
    public class ChangeMilestoneParticipationStatusEndpoint : Endpoint<ChangeMilestoneParticipationStatusCommand>
    {
        private readonly ChangeMilestoneParticipationStatusHandler _handler;

        public ChangeMilestoneParticipationStatusEndpoint(ChangeMilestoneParticipationStatusHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Put("/milestone-participations/status");
            Roles("Administrator");
            Validator<ChangeMilestoneParticipationStatusValidator>();
            Summary(s =>
            {
                s.Summary = "Changes the active status of a milestone participation.";
                s.Description = "Activates or deactivates a participation. Cannot deactivate if the participation is already paid.";
                s.ExampleRequest = new ChangeMilestoneParticipationStatusCommand
                {
                    IsActive = false
                };
            });
        }

        public override async Task HandleAsync(ChangeMilestoneParticipationStatusCommand req, CancellationToken ct)
        {
            await _handler.HandleAsync(req);
            await Send.OkAsync();
        }
    }
}