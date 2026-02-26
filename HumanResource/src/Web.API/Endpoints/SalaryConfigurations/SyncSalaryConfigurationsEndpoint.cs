using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using Application.SalaryConfigurations.Handlers;

namespace Web.API.Endpoints.SalaryConfigurations
{
    public class SyncSalaryConfigurationsEndpoint : EndpointWithoutRequest
    {
        private readonly SyncSalaryConfigurationsHandler _handler;

        public SyncSalaryConfigurationsEndpoint(SyncSalaryConfigurationsHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Post("/salary-configurations/sync");
            Roles("Administrator");
            Summary(s =>
            {
                s.Summary = "Sync salary configurations";
                s.Description = "Ensures all employee roles have corresponding salary configurations.";
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var created = await _handler.HandleAsync(ct);
            await Send.OkAsync(new
            {
                Message = "Salary configurations synchronized successfully.",
                Created = created
            }, ct);
        }
    }
}
