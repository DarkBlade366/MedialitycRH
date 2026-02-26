using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using Application.SalaryConfigurations.Commands;
using Application.SalaryConfigurations.Handlers;

namespace Web.API.Endpoints.SalaryConfigurations
{
    public class UpdateSalaryConfigurationEndpoint : Endpoint<UpdateSalaryConfigurationCommand>
    {
        private readonly UpdateSalaryConfigurationHandler _handler;

        public UpdateSalaryConfigurationEndpoint(UpdateSalaryConfigurationHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Post("/salary-configurations/update");
            Roles("Administrator");
            Summary(s =>
            {
                s.Summary = "Update salary configuration";
                s.Description = "Updates the hourly rate for a specific employee role.";
                s.ExampleRequest = new UpdateSalaryConfigurationCommand
                {
                    Role = Domain.Enums.EmployeeRole.Employee,
                    NewHourlyRate = 50.00m
                };
            });
        }

        public override async Task HandleAsync(UpdateSalaryConfigurationCommand command, CancellationToken ct)
        {
            await _handler.HandleAsync(command, ct);
            await Send.NoContentAsync(ct);
        }
    }
}