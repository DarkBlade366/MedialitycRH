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
    public class UseVacationEndpoint : Endpoint<UseVacationCommand>
    {
        private readonly UseVacationHandler _handler;

        public UseVacationEndpoint(UseVacationHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Post("/employees/{EmployeeId}/use-vacation");
            Roles("Administrator, HumanResource, ProjectManager, Employee");
            Validator<UseVacationCommandValidator>();
            Summary(s =>
            {
                s.Summary = "Register vacation usage for an employee";
                s.Description = "Registers that an employee has used a number of vacation days.";
                s.ExampleRequest = new 
                { 
                    EmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000000"), 
                    Days = 3m 
                };
            });
        }

        public override async Task HandleAsync(UseVacationCommand req, CancellationToken ct)
        {
            await _handler.Handle(req, ct);
            await Send.OkAsync(ct);
        }
    }
}