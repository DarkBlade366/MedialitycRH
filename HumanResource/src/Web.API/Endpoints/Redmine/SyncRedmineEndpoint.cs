using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Redmine;
using FastEndpoints;

namespace Web.API.Endpoints.Redmine
{
    public class SyncRedmineEndpoint : EndpointWithoutRequest
    {
        private readonly SyncRedmineTimeEntriesHandler _handler;

        public SyncRedmineEndpoint(SyncRedmineTimeEntriesHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Post("/redmine/sync");
            Roles("Administrator");
            Summary(s =>
            {
                s.Summary = "Synchronize Redmine time entries";
                s.Description = "Synchronizes time entries from Redmine into local database";
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var from = DateTime.UtcNow.AddDays(-30);
            var to = DateTime.UtcNow;

            await _handler.Handle(from, to);
            await Send.OkAsync("Synchronization completed", ct);
        }
    }
}