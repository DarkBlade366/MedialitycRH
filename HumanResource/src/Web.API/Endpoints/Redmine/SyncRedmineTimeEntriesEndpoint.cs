using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Redmine;
using Application.Features.Redmine.Handlers;
using FastEndpoints;

namespace Web.API.Endpoints.Redmine
{
    public class SyncRedmineTimeEntriesEndpoint : EndpointWithoutRequest
    {
        private readonly SyncRedmineTimeEntriesHandler _handler;

        public SyncRedmineTimeEntriesEndpoint(SyncRedmineTimeEntriesHandler handler)
        {
            _handler = handler;
        }   

        public override void Configure()
        {
            Post("/redmine/sync-time-entries");
            Roles("Administrator");
            Summary(s =>
            {
                s.Summary = "Synchronize Redmine time entries.";
                s.Description = "Creates local time entries from Redmine if they don't exist. Does not update or delete existing entries.";
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var from = DateTime.UtcNow.AddDays(-30);
            var to = DateTime.UtcNow;

            var created = await _handler.Handle(from, to);
            await Send.OkAsync(new
            {
                Message = "Time entry synchronization completed",
                CreatedTimeEntries = created
            }, ct);
        }
    }
}