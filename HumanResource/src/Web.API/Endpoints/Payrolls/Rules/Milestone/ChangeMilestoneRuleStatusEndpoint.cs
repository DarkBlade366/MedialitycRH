using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Milestones.Commands;
using Application.Features.Payrolls.Rules.Milestones.DTOs;
using Application.Features.Payrolls.Rules.Milestones.Handlers;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Milestone
{
    public class ChangeMilestoneRuleStatusEndpoint : Endpoint<ChangeMilestoneRuleStatusCommand>
    {
        private readonly ChangeMilestoneRuleStatusHandler _handler;

        public ChangeMilestoneRuleStatusEndpoint(ChangeMilestoneRuleStatusHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Put("/milestone-rules/{id:guid}/status");
            Roles("Administrator");
            Summary(s =>
            {
                s.Summary = "Change the status of a milestone rule.";
                s.Description = "Activate or deactivate a milestone rule by changing its status.";
                s.ExampleRequest = new ChangeMilestoneRuleStatusCommand
                {
                    IsActive = true
                };
            });
        }

        public override async Task HandleAsync(ChangeMilestoneRuleStatusCommand req, CancellationToken ct)
        {
            var id = Route<Guid>("id");

            await _handler.HandleAsync(req);
            await Send.NoContentAsync();
        }
    }
}