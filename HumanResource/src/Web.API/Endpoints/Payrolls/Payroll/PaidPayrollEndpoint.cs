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
    public class PaidPayrollEndpoint : EndpointWithoutRequest<PayrollResponse>
    {
        private readonly PaidPayrollHandler _handler;

        public PaidPayrollEndpoint (PaidPayrollHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Post("/payrolls/paid/{id}");
            Roles("Administrator, HumanResourse");
            Summary(s =>
            {
                s.Summary = "Paid payroll for an employee.";
                s.Description = "Paid a payroll for the specified employee.";
                s.ExampleRequest = new PaidPayrollCommand
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000000")
                };
            });
        }

        public override async Task HandleAsync(
            CancellationToken cancellationToken)
        {
            var id = Route<Guid>("id");

            var result = await _handler.Handle(new PaidPayrollCommand {Id = id}, cancellationToken);
            await Send.OkAsync(result, cancellationToken);
        }
    }
}