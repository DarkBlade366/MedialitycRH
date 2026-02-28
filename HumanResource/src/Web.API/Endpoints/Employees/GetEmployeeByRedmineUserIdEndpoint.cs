using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Employees.Queries;
using Application.Features.Employees.DTOs;
using Application.Features.Employees.Handlers;
using Application.Features.Employees.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Employees
{
    public class GetEmployeeByRedmineUserIdEndpoint : Endpoint<GetEmployeeByRedmineUserIdQuery, GetEmployeeByIdResponse>
    {
        private readonly GetEmployeeByRedmineUserIdHandler _handler;

        public GetEmployeeByRedmineUserIdEndpoint(GetEmployeeByRedmineUserIdHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/employees/redmine/{redmineUserId}");
            Roles("Administrator", "HumanResources");
            Validator<GetEmployeeByRedmineUserIdValidation>();
            Summary(s =>
            {
                s.Summary = "Get employee by RedmineUserId.";
                s.Description = "Returns the details of an employee by their RedmineUserId.";
                s.ExampleRequest = new GetEmployeeByRedmineUserIdQuery
                {
                    RedmineUserId = 100
                };
            });
        }

        public override async Task HandleAsync(GetEmployeeByRedmineUserIdQuery req, CancellationToken ct)
        {
            var result = await _handler.Handle(req, ct);
            await Send.OkAsync(result, ct);
        }
    }
}