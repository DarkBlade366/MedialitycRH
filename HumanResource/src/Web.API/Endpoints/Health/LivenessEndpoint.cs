using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;

namespace Web.API.Endpoints.Health
{
    public class LivenessEndpoint : EndpointWithoutRequest
    {
        public override void Configure()
        {
            Get("/health/live");
            AllowAnonymous();
            Description(b => b
                .WithName("Liveness")
                .WithTags("Health"));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            await Send.OkAsync(new { status = "alive" }, ct);
        }
    }
}
