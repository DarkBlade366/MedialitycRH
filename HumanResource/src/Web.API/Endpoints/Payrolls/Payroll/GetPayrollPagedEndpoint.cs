using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Payroll.DTOs;
using Application.Features.Payrolls.Payroll.Handlers;
using Application.Features.Payrolls.Payroll.Queries;
using Application.Features.Payrolls.Payroll.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Payroll
{
    public class GetPayrollPagedEndpoint 
        : Endpoint<GetPayrollPagedQuery, PagedResponse<PayrollResponse>>
    {
        private readonly GetPayrollPagedHandler _handler;

        public GetPayrollPagedEndpoint(GetPayrollPagedHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/payrolls");
            Roles("Administrator");
            Validator<GetPayrollPagedQueryValidator>();
            Summary(s =>
            {
                s.Summary = "Get a paginated list of payrolls.";
                s.Description = "Retrieve a paginated list of payrolls with optional filtering and sorting.";
                s.ExampleRequest = new GetPayrollPagedQuery
                {
                    Page = 1,
                    PageSize = 10
                };
            });
        }

        public override async Task HandleAsync(
            GetPayrollPagedQuery req,
            CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);

            await Send.OkAsync(result);
        }
    }
}