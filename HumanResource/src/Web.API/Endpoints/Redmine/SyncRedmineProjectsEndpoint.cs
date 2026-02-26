using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Redmine.Handlers;
using FastEndpoints;

namespace Web.API.Endpoints.Redmine
{
    public class SyncRedmineProjectsEndpoint : EndpointWithoutRequest
    {
        private readonly SyncRedmineProjectsHandler _handler;

        public SyncRedmineProjectsEndpoint(SyncRedmineProjectsHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Post("/redmine/sync-projects");
            Roles("Administrator", "HumanResources");
            Summary(s =>
            {
                s.Summary = "Synchronize projects from Redmine";
                s.Description = "Fetches projects from Redmine and updates the local database. Returns the number of projects created.";
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var created = await _handler.Handle();
            await Send.OkAsync(new { Created = created }, ct);
        }
    }
}