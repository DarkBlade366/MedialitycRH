using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Employees.Commands;
using Application.Features.Employees.Handlers;
using Application.Features.Employees.Validations;
using Domain.Features.Employees.Enums;
using FastEndpoints;

namespace Web.API.Endpoints.Employees
{
    public class CreateEmployeeEndpoint : Endpoint<CreateEmployeeCommand, Guid>
    {
        private readonly CreateEmployeeHandler _handler;

        public CreateEmployeeEndpoint(CreateEmployeeHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Post("/employees");
            Roles("Administrator");
            Validator<CreateEmployeeValidation>();
            Summary(s =>
            {
                s.Summary = "Create a new employee.";
                s.Description = "Creates a new employee with the provided data.";
                s.ExampleRequest = new CreateEmployeeCommand
                {
                    FullName = "Juan Pérez",
                    Email = "juan.perez@gmail.com",
                    Password = "XXXXXXXX",
                    RedmineUserId = 100,
                    Role = EmployeeRole.Employee
                };
            });
        }

        public override async Task HandleAsync(CreateEmployeeCommand req, CancellationToken ct)
        {
            var id = await _handler.Handle(req, ct);
            await Send.OkAsync(id, ct);
        }
    }
}