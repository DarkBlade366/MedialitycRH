using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Application.Employees.Commands;
using Application.Employees.Handlers;
using Application.Employees.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Employees
{
    public class ChangeEmployeeRedmineUserIdEndpoint : Endpoint<ChangeEmployeeRedmineUserIdCommand>
    {
        private readonly ChangeEmployeeRedmineUserIdHandler _handler;

        public ChangeEmployeeRedmineUserIdEndpoint(ChangeEmployeeRedmineUserIdHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Put("/employees/{id}/redmine");
            Roles("Administrator");
            Validator<ChangeEmployeeRedmineUserIdValidator>();
            Summary(s =>
            {
                s.Summary = "Set Redmine User Id";
                s.Description = "Assigns or updates the Redmine user ID for an employee.";
                s.ExampleRequest = new ChangeEmployeeRedmineUserIdCommand
                {
                    RedmineUserId = 12345
                };
            });
        }

        public override async Task HandleAsync(ChangeEmployeeRedmineUserIdCommand req, CancellationToken ct)
        {
            await _handler.Handle(req);
            await Send.NoContentAsync(ct);
        }
    }
}