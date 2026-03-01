using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Rules.Aguinaldo.DTOs;
using Application.Features.Payrolls.Rules.Aguinaldo.Handlers;
using Application.Features.Payrolls.Rules.Aguinaldo.Queries;
using Application.Features.Payrolls.Rules.Aguinaldo.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Aguinaldos
{
    public class GetAguinaldoRulesPagedEndpoint
        : Endpoint<GetAguinaldoRulesPagedQuery, PagedResponse<AguinaldoRuleResponse>>
    {
        private readonly GetAguinaldoRulesPagedHandler _handler;

        public GetAguinaldoRulesPagedEndpoint(GetAguinaldoRulesPagedHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/aguinaldo-rules");
            Roles("Administrator");
            Validator<GetAguinaldoRulesPagedValidator>();
            Summary(s =>
            {
                s.Summary = "Get a paginated list of aguinaldo rules.";
                s.Description = "Retrieve a paginated list of aguinaldo rules with optional filtering and sorting.";
                s.ExampleRequest = new GetAguinaldoRulesPagedQuery
                {
                    Page = 1,
                    PageSize = 10
                };
            });
        }

        public override async Task HandleAsync(
            GetAguinaldoRulesPagedQuery req,
            CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);

            await Send.OkAsync(result);
        }
    }
}