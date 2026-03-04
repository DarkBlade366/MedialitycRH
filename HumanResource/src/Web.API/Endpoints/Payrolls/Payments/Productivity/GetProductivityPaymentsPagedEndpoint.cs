using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Payments.Productivity.DTOs;
using Application.Features.Payrolls.Payments.Productivity.Handlers;
using Application.Features.Payrolls.Payments.Productivity.Queries;
using Application.Features.Payrolls.Payments.Productivity.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Payments.Productivity
{
    public class GetProductivityPaymentsPagedEndpoint
        : Endpoint<GetProductivityPaymentsPagedQuery, PagedResponse<ProductivityPaymentResponse>>
    {
        private readonly GetProductivityPaymentsPagedHandler _handler;

        public GetProductivityPaymentsPagedEndpoint(GetProductivityPaymentsPagedHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/productivity-payments");
            Roles("Administrator");
            Validator<GetProductivityPaymentsPagedValidator>();
            Summary(s =>
            {
                s.Summary = "Get a paginated list of productivity payment records.";
                s.Description = "Retrieve historical productivity payments with optional filtering and pagination.";
                s.ExampleRequest = new GetProductivityPaymentsPagedQuery
                {
                    Page = 1,
                    PageSize = 10
                };
            });
        }

        public override async Task HandleAsync(
            GetProductivityPaymentsPagedQuery req,
            CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result);
        }
    }
}
