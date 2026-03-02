using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Aguinaldo.DTOs;
using Application.Features.Payrolls.Rules.Aguinaldo.Handlers;
using Application.Features.Payrolls.Rules.Aguinaldo.Queries;
using Application.Features.Payrolls.Rules.Aguinaldo.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Aguinaldo
{
    public class GetAguinaldoRuleByIdEndpoint : Endpoint<GetAguinaldoRuleByIdQuery, AguinaldoRuleResponse>
    {
        private readonly GetAguinaldoRuleByIdHandler _handler;
        public GetAguinaldoRuleByIdEndpoint(GetAguinaldoRuleByIdHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/aguinaldo-rules/{id}");
            Roles("Administrator");
            Validator<GetAguinaldoRuleByIdValidator>();
            Summary(s =>
            {
                s.Summary = "Get an aguinaldo rule by its ID.";
                s.Description = "Retrieve the details of a specific aguinaldo rule using its unique identifier.";
                s.ExampleRequest = new
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000000")
                };
            });
        }

        public override async Task HandleAsync(GetAguinaldoRuleByIdQuery request, CancellationToken ct)
        {
            var result = await _handler.HandleAsync(request);
            
            if (result == null)
                await Send.NotFoundAsync(ct);
            else
                await Send.OkAsync(result, ct);
        }
    }
}
