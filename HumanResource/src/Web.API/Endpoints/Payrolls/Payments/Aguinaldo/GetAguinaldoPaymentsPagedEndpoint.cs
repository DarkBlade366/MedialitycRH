using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Payments.Aguinaldo.DTOs;
using Application.Features.Payrolls.Payments.Aguinaldo.Handlers;
using Application.Features.Payrolls.Payments.Aguinaldo.Queries;
using Application.Features.Payrolls.Payments.Aguinaldo.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Payments.Aguinaldo
{
    public class GetAguinaldoPaymentsPagedEndpoint
        : Endpoint<GetAguinaldoPaymentsPagedQuery, PagedResponse<AguinaldoPaymentResponse>>
    {
        private readonly GetAguinaldoPaymentsPagedHandler _handler;

        public GetAguinaldoPaymentsPagedEndpoint(GetAguinaldoPaymentsPagedHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/aguinaldo-payments");
            Roles("Administrator");
            Validator<GetAguinaldoPaymentsPagedValidator>();
            Summary(s =>
            {
                s.Summary = "Get a paginated list of aguinaldo payment records.";
                s.Description = "Retrieve historical aguinaldo payments with optional filtering and pagination.";
                s.ExampleRequest = new GetAguinaldoPaymentsPagedQuery
                {
                    Page = 1,
                    PageSize = 10
                };
            });
        }

        public override async Task HandleAsync(
            GetAguinaldoPaymentsPagedQuery req,
            CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result);
        }
    }
}
