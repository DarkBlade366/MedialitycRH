using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using Application.Features.Redmine.Handlers;

namespace Web.API.Endpoints.Redmine
{
    public class SyncRedmineMilestonesEndpoint : EndpointWithoutRequest
    {
        private readonly SyncRedmineMilestonesHandler _handler;

        public SyncRedmineMilestonesEndpoint(SyncRedmineMilestonesHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Post("/redmine/sync-milestones");
            Roles("Administrator", "HumanResources");
            Summary(s =>
            {
                s.Summary = "Synchronize all milestones from Redmine";
                s.Description = "Fetches all milestones from Redmine for all projects, updates the local database, and returns the number created.";
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var created = await _handler.Handle(ct);

            await Send.OkAsync(new
            {
                Message = "Milestones synchronization completed",
                CreatedMilestones = created
            }, ct);
        }
    }
}