using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Payroll.Commands;
using Application.Features.Payrolls.Payroll.DTOs;
using Application.Features.Payrolls.Payroll.Handlers;
using Application.Features.Payrolls.Payroll.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Payroll
{
    public class CreatePayrollEndpoint : Endpoint<CreatePayrollCommand, PayrollResponse>
    {
        private readonly CreatePayrollHandler _handler;

        public CreatePayrollEndpoint(CreatePayrollHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Post("/payrolls");
            Roles("Administrator, HumanResourse");
            Validator<CreatePayrollCommandValidator>();
            Summary(s =>
            {
                s.Summary = "Create payroll for an employee.";
                s.Description = "Generates a payroll for the specified employee and period, applying all active rules.";
                s.ExampleRequest = new CreatePayrollCommand
                {
                    periodStart = DateTime.UtcNow.AddMonths(-1),
                    periodEnd = DateTime.UtcNow
                };
            });
        }

        public override async Task HandleAsync(
            CreatePayrollCommand req,
            CancellationToken ct)
        {
            var result = await _handler.Handle(req, ct);
            await Send.OkAsync(result, ct);
        }
    }
}