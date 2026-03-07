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
    public class GetProductivityTrendEndpoint : Endpoint<GetProductivityTrendQuery, List<ProductivityTrendDto>>
    {
        private readonly GetProductivityTrendHandler _handler;

        public GetProductivityTrendEndpoint(GetProductivityTrendHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/analytics/productivity-trend/{employeeId}");
            Roles("Administrator", "HumanResources", "ProjectManager");
            Validator<GetProductivityTrendValidator>();
            Summary(s =>
            {
                s.Summary = "Get productivity trend (monthly) for an employee.";
                s.Description = "Returns the weighted productivity metric for each month in the given range.";
                s.ExampleRequest = new GetProductivityTrendQuery
                {
                    EmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    From = DateTime.Parse("2024-01-01"),
                    To = DateTime.Parse("2024-03-31")
                };
            });
        }

        public override async Task HandleAsync(GetProductivityTrendQuery req, CancellationToken ct)
        {
            var result = await _handler.Handle(req, ct);
            await Send.OkAsync(result, ct);
        }
    }
}