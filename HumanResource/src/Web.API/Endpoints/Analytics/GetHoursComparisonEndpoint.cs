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
    public class GetHoursComparisonEndpoint : Endpoint<GetHoursComparisonQuery, HoursComparisonDto>
    {
        private readonly GetHoursComparisonHandler _handler;

        public GetHoursComparisonEndpoint(GetHoursComparisonHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/analytics/hours-comparison/{employeeId}");
            Roles("Administrator", "HumanResources", "ProjectManager");
            Validator<GetHoursComparisonValidator>();
            Summary(s =>
            {
                s.Summary = "Compare registered hours vs expected hours for an employee.";
                s.Description = "This endpoint provides a comprehensive comparison between registered hours from time entries and expected hours (160h per employee) within a specified period. It returns total registered hours, expected hours, variance percentage, and detailed breakdown by employee.";
                s.RequestParam(r => r.EmployeeId, "Employee unique identifier (GUID)");
                s.RequestParam(r => r.PeriodStart, "Start date of the analysis period (YYYY-MM-DD)");
                s.RequestParam(r => r.PeriodEnd, "End date of the analysis period (YYYY-MM-DD)");
                s.ExampleRequest = new GetHoursComparisonQuery
                {
                    EmployeeId = Guid.Parse("12345678-1234-1234-1234-123456789012"),
                    PeriodStart = DateTime.Parse("2024-01-01"),
                    PeriodEnd = DateTime.Parse("2024-01-31")
                };
            });
        }

        public override async Task HandleAsync(GetHoursComparisonQuery req, CancellationToken ct)
        {
            var result = await _handler.Handle(req, ct);
            await Send.OkAsync(result, ct);
        }
    }
}