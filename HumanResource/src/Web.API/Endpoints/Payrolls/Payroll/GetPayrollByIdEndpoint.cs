using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Payroll.DTOs;
using Application.Features.Payrolls.Payroll.Handlers;
using Application.Features.Payrolls.Payroll.Queries;
using Application.Features.Payrolls.Payroll.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Payroll
{
    public class GetPayrollByIdEndpoint : Endpoint<GetPayrollByIdQuery, PayrollResponse>
    {
        private readonly GetPayrollByIdHandler _handler;
        public GetPayrollByIdEndpoint(GetPayrollByIdHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/payrolls/{id}");
            Roles("Administrator");
            Validator<GetPayrollByIdQueryValidator>();
            Summary(s =>
            {
                s.Summary = "Get an payroll by its ID.";
                s.Description = "Retrieve the details of a specific payroll using its unique identifier.";
                s.ExampleRequest = new
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000000")
                };
            });
        }

        public override async Task HandleAsync(GetPayrollByIdQuery request, CancellationToken ct)
        {
            var result = await _handler.HandleAsync(request);
            
            if (result == null)
                await Send.NotFoundAsync(ct);
            else
                await Send.OkAsync(result, ct);
        }
    }
}