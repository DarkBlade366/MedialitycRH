using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Employees.DTOs;
using Application.Features.Employees.Handlers;
using Application.Features.Employees.Queries;
using Application.Features.Employees.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Employees
{
    public class GetEmployeeByIdEndpoint : Endpoint<GetEmployeeByIdQuery, GetEmployeeByIdResponse>
    {
        private readonly GetEmployeeByIdHandler _handler;

        public GetEmployeeByIdEndpoint(GetEmployeeByIdHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/employees/{id:guid}");
            Roles("Administrator", "HumanResources");
            Validator<GetEmployeeByIdValidation>();
            Summary(s =>
            {
                s.Summary = "Get employee by ID.";
                s.Description = "Returns the details of a specific employee.";
                s.ExampleRequest = new GetEmployeeByIdQuery
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000000")
                };
            });
        }

        public override async Task HandleAsync(GetEmployeeByIdQuery req, CancellationToken ct)
        {
            var result = await _handler.Handle(req);
            await Send.OkAsync(result, ct);
        }
    }
}