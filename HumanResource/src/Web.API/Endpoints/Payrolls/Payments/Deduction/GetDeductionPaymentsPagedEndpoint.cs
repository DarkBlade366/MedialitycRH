using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Payments.Deduction.DTOs;
using Application.Features.Payrolls.Payments.Deduction.Handlers;
using Application.Features.Payrolls.Payments.Deduction.Queries;
using Application.Features.Payrolls.Payments.Deduction.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Payments.Deduction
{
    public class GetDeductionPaymentsPagedEndpoint
        : Endpoint<GetDeductionPaymentsPagedQuery, PagedResponse<DeductionPaymentResponse>>
    {
        private readonly GetDeductionPaymentsPagedHandler _handler;

        public GetDeductionPaymentsPagedEndpoint(GetDeductionPaymentsPagedHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/deduction-payments");
            Roles("Administrator");
            Validator<GetDeductionPaymentsPagedValidator>();
            Summary(s =>
            {
                s.Summary = "Get a paginated list of deduction payment records.";
                s.Description = "Retrieve historical deduction payments with optional filtering and pagination.";
                s.ExampleRequest = new GetDeductionPaymentsPagedQuery
                {
                    Page = 1,
                    PageSize = 10
                };
            });
        }

        public override async Task HandleAsync(
            GetDeductionPaymentsPagedQuery req,
            CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result);
        }
    }
}
