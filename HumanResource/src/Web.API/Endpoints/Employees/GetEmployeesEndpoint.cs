using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Employees.DTOs;
using Application.Employees.Handlers;
using Application.Employees.Queries;
using Application.Employees.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Employees
{
    public class GetEmployeesEndpoint : Endpoint<GetEmployeesQuery, PagedResponse<EmployeeListItemDto>>
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
                s.Summary = "Obtener empleados paginados";
                s.Description = "Devuelve la lista de empleados con paginación.";
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