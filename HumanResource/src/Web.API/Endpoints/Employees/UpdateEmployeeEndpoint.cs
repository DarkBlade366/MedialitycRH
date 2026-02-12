using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Employees.Commands;
using Application.Employees.Handlers;
using Application.Employees.Validations;
using FastEndpoints;
using Domain.Enums;

namespace Web.API.Endpoints.Employees
{
    public class UpdateEmployeeEndpoint : Endpoint<UpdateEmployeeCommand>
    {
        private readonly UpdateEmployeeHandler _handler;

        public UpdateEmployeeEndpoint(UpdateEmployeeHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Put("/employees/{id}");
            Roles("Administrator");
            Validator<UpdateEmployeeValidation>();
            Summary(s =>
            {
                s.Summary = "Update employee";
                s.Description = "Updates employee basic information.";
                s.ExampleRequest = new UpdateEmployeeCommand
                {
                    FullName = "Juan Pérez",
                    Email = "juan.perez@gmail.com",
                    Role = EmployeeRole.Employee
                };
            });
        }

        public override async Task HandleAsync(UpdateEmployeeCommand req, CancellationToken ct)
        {
            await _handler.Handle(req);
            await Send.NoContentAsync(ct);
        }
    }
}