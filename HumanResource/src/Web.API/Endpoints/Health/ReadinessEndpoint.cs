using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
namespace Web.API.Endpoints.Health
{
    public class ReadinessEndpoint : EndpointWithoutRequest
    {
        public override void Configure()
        {
            Get("/health/ready");
            AllowAnonymous();
            Description(b => b
                .WithName("Readiness")
                .WithTags("Health"));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            await Send.OkAsync(new { status = "ready" }, ct);
        }
    }
}