using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Redmine;
using FastEndpoints;

namespace Web.API.Endpoints.Redmine
{
    public class SyncRedmineUsersEndpoint : EndpointWithoutRequest
    {
        private readonly SyncRedmineUsersHandler _handler;

        public SyncRedmineUsersEndpoint(SyncRedmineUsersHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Post("/redmine/sync-users");
            Roles("Administrator");
            Summary(s =>
            {
                s.Summary = "Synchronize Redmine users";
                s.Description = "Creates local employees from Redmine users if they do not exist.";
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var created = await _handler.Handle();

            await Send.OkAsync(new
{
                Message = "User synchronization completed",
                CreatedEmployees = created
            }, ct);
        }
    }
}
