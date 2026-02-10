using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Employees.DTOs;
using Application.Employees.Handlers;
using Application.Employees.Queries;
using Application.Employees.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Employees
{
    public class GetEmployeeByIdEndpoint : Endpoint<GetEmployeeByIdQuery, EmployeeDetailDto>
    {
        private readonly GetEmployeeByIdHandler _handler;

        public GetEmployeeByIdEndpoint(GetEmployeeByIdHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/employees/{id}");
            Roles("Administrator", "HumanResources");
            Validator<GetEmployeeByIdValidation>();

            Summary(s =>
            {
                s.Summary = "Obtener empleado por ID";
                s.Description = "Devuelve el detalle de un empleado específico.";
                s.ExampleRequest = new GetEmployeeByIdQuery
                {
                    Id = Guid.NewGuid()
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