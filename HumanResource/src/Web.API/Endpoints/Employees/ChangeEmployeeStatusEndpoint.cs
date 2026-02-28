using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Employees.Commands;
using Application.Features.Employees.Handlers;
using Application.Features.Employees.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Employees
{
    public class ChangeEmployeeStatusEndpoint : Endpoint<ChangeEmployeeStatusCommand>
    {
        private readonly ChangeEmployeeStatusHandler _handler;

        public ChangeEmployeeStatusEndpoint(ChangeEmployeeStatusHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Put("/employees/{id}/status");
            Roles("Administrator");
            Validator<ChangeEmployeeStatusValidation>();
            Summary(s =>
            {
                s.Summary = "Change employee status.";
                s.Description = "Logically changes an employee's status (IsActive = true/false).";
                s.ExampleRequest = new ChangeEmployeeStatusCommand
                {
                    IsActive = false
                };
            });
        }

        public override async Task HandleAsync(ChangeEmployeeStatusCommand req, CancellationToken ct)
        {
            await _handler.Handle(req, ct);
            await Send.NoContentAsync(ct);
        }
    }
}