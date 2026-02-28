using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Employees.DTOs;
using Application.Features.Employees.Handlers;
using Application.Features.Employees.Queries;
using Application.Features.Employees.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Employees
{
    public class GetEmployeesEndpoint : Endpoint<GetEmployeesQuery, PagedResponse<GetEmployeesResponse>>
    {
        private readonly GetEmployeesHandler _handler;

        public GetEmployeesEndpoint(GetEmployeesHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/employees");
            Roles("Administrator", "HumanResources");
            Validator<GetEmployeesValidation>();
            Summary(s =>
            {
                s.Summary = "Get paged employees.";
                s.Description = "Returns the list of employees with pagination.";
                s.ExampleRequest = new GetEmployeesQuery
                {
                    Page = 1,
                    PageSize = 10
                };
            });
        }

        public override async Task HandleAsync(GetEmployeesQuery req, CancellationToken ct)
        {
            var result = await _handler.Handle(req);
            await Send.OkAsync(result, ct);
        }
    }
}