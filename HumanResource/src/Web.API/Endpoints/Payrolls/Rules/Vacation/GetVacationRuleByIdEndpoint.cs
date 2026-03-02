using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Vacation.DTOs;
using Application.Features.Payrolls.Rules.Vacation.Handlers;
using Application.Features.Payrolls.Rules.Vacation.Queries;
using Application.Features.Payrolls.Rules.Vacation.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Vacation
{
    public class GetVacationRuleByIdEndpoint : Endpoint<GetVacationRuleByIdQuery, VacationRuleResponse>
    {
        private readonly GetVacationRuleByIdHandler _handler;

        public GetVacationRuleByIdEndpoint(GetVacationRuleByIdHandler handler) => _handler = handler;

        public override void Configure()
        {
            Get("/vacation-rules/{id}");
            Roles("Administrator");
            Validator<GetVacationRuleByIdValidator>();
            Summary(s =>
            {
                s.Summary = "Retrieve vacation rule by Id.";
                s.Description = "Fetch a specific vacation rule including its accrual rate and pay-on-use flag.";
                s.ExampleRequest = new GetVacationRuleByIdQuery
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000000")
                };
            });
        }

        public override async Task HandleAsync(GetVacationRuleByIdQuery req, CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);

            if (result == null) 
                await Send.NotFoundAsync(ct);
            else 
                await Send.OkAsync(result, ct);
        }
    }
}