using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Analytics.DTOs;
using Application.Features.Analytics.Handlers;
using Application.Features.Analytics.Queries;
using Application.Features.Analytics.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Analytics
{
    public class GetProjectCostsEndpoint : Endpoint<GetProjectCostsQuery, List<ProjectCostDto>>
    {
        private readonly GetProjectCostsHandler _handler;

        public GetProjectCostsEndpoint(GetProjectCostsHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/analytics/project-costs");
            Roles("Administrator", "HumanResources", "ProjectManager");
            Validator<GetProjectCostsValidator>();
            Summary(s =>
            {
                s.Summary = "Get project cost summary for a period.";
                s.Description = "Returns total hours and estimated cost per project based on approved time entries.";
                s.ExampleRequest = new GetProjectCostsQuery
                {
                    PeriodStart = DateTime.Parse("2024-01-01"),
                    PeriodEnd = DateTime.Parse("2024-01-31"),
                    ProjectId = 123
                };
            });
        }

        public override async Task HandleAsync(GetProjectCostsQuery req, CancellationToken ct)
        {
            var result = await _handler.Handle(req, ct);
            await Send.OkAsync(result, ct);
        }
    }
}