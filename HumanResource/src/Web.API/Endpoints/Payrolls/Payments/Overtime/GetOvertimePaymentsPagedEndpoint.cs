using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Payments.Overtime.DTOs;
using Application.Features.Payrolls.Payments.Overtime.Handlers;
using Application.Features.Payrolls.Payments.Overtime.Queries;
using Application.Features.Payrolls.Payments.Overtime.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Payments.Overtime
{
    public class GetOvertimePaymentsPagedEndpoint
        : Endpoint<GetOvertimePaymentsPagedQuery, PagedResponse<OvertimePaymentResponse>>
    {
        private readonly GetOvertimePaymentsPagedHandler _handler;

        public GetOvertimePaymentsPagedEndpoint(GetOvertimePaymentsPagedHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/overtime-payments");
            Roles("Administrator");
            Validator<GetOvertimePaymentsPagedValidator>();
            Summary(s =>
            {
                s.Summary = "Get a paginated list of overtime payment records.";
                s.Description = "Retrieve historical overtime payments with optional filtering and pagination.";
                s.ExampleRequest = new GetOvertimePaymentsPagedQuery
                {
                    Page = 1,
                    PageSize = 10
                };
            });
        }

        public override async Task HandleAsync(
            GetOvertimePaymentsPagedQuery req,
            CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result);
        }
    }
}
