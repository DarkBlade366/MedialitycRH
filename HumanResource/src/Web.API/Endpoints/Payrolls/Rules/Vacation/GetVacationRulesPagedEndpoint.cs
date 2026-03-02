using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Rules.Vacation.DTOs;
using Application.Features.Payrolls.Rules.Vacation.Handlers;
using Application.Features.Payrolls.Rules.Vacation.Queries;
using Application.Features.Payrolls.Rules.Vacation.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Vacation
{
    public class GetVacationRulesPagedEndpoint
        : Endpoint<GetVacationRulesPagedQuery, PagedResponse<VacationRuleResponse>>
    {
        private readonly GetVacationRulesPagedHandler _handler;

        public GetVacationRulesPagedEndpoint(GetVacationRulesPagedHandler handler) => _handler = handler;

        public override void Configure()
        {
            Get("/vacation-rules");
            Roles("Administrator");
            Validator<GetVacationRulesPagedValidator>();
            Summary(s =>
            {
                s.Summary = "Get a paginated list of vacation rules.";
                s.Description = "Supports optional filtering by active status and PayVacationOnUse flag.";
                s.ExampleRequest = new GetVacationRulesPagedQuery
                {
                    Page = 1,
                    PageSize = 10
                };
            });
        }

        public override async Task HandleAsync(GetVacationRulesPagedQuery req, CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result, ct);
        }
    }
}