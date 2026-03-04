using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Payments.Milestone.DTOs;
using Application.Features.Payrolls.Payments.Milestone.Handlers;
using Application.Features.Payrolls.Payments.Milestone.Queries;
using Application.Features.Payrolls.Payments.Milestone.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Payments.Milestone
{
    public class GetMilestonePaymentsPagedEndpoint
        : Endpoint<GetMilestonePaymentsPagedQuery, PagedResponse<MilestonePaymentResponse>>
    {
        private readonly GetMilestonePaymentsPagedHandler _handler;

        public GetMilestonePaymentsPagedEndpoint(GetMilestonePaymentsPagedHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/milestone-payments");
            Roles("Administrator");
            Validator<GetMilestonePaymentsPagedValidator>();
            Summary(s =>
            {
                s.Summary = "Get a paginated list of milestone payment records.";
                s.Description = "Retrieve historical milestone payments with optional filtering and pagination.";
                s.ExampleRequest = new GetMilestonePaymentsPagedQuery
                {
                    Page = 1,
                    PageSize = 10
                };
            });
        }

        public override async Task HandleAsync(
            GetMilestonePaymentsPagedQuery req,
            CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result);
        }
    }
}
