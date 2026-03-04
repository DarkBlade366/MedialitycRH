using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Payments.Vacation.DTOs;
using Application.Features.Payrolls.Payments.Vacation.Handlers;
using Application.Features.Payrolls.Payments.Vacation.Queries;
using Application.Features.Payrolls.Payments.Vacation.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Payments.Vacation
{
    public class GetVacationPaymentsPagedEndpoint
        : Endpoint<GetVacationPaymentsPagedQuery, PagedResponse<VacationPaymentResponse>>
    {
        private readonly GetVacationPaymentsPagedHandler _handler;

        public GetVacationPaymentsPagedEndpoint(GetVacationPaymentsPagedHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/vacation-payments");
            Roles("Administrator");
            Validator<GetVacationPaymentsPagedValidator>();
            Summary(s =>
            {
                s.Summary = "Get a paginated list of vacation payment records.";
                s.Description = "Retrieve historical vacation payments with optional filtering and pagination.";
                s.ExampleRequest = new GetVacationPaymentsPagedQuery
                {
                    Page = 1,
                    PageSize = 10
                };
            });
        }

        public override async Task HandleAsync(
            GetVacationPaymentsPagedQuery req,
            CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result);
        }
    }
}
