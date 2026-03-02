using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Rules.Productivity.DTOs;
using Application.Features.Payrolls.Rules.Productivity.Handlers;
using Application.Features.Payrolls.Rules.Productivity.Queries;
using Application.Features.Payrolls.Rules.Productivity.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Productivity
{
    public class GetProductivityRulesPagedEndpoint
        : Endpoint<GetProductivityRulesPagedQuery, PagedResponse<ProductivityRuleResponse>>
    {
        private readonly GetProductivityRulesPagedHandler _handler;

        public GetProductivityRulesPagedEndpoint(
            GetProductivityRulesPagedHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/productivity-rules");
            Roles("Administrator");
            Validator<GetProductivityRulesPagedValidator>();
            Summary(s =>
            {
                s.Summary = "Get paginated productivity rules.";
                s.Description = "Retrieve a paginated list of productivity rules with optional filters.";
                s.ExampleRequest = new GetProductivityRulesPagedQuery
                {
                    Page = 1,
                    PageSize = 10
                };
            });
        }

        public override async Task HandleAsync(
            GetProductivityRulesPagedQuery req,
            CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result, ct);
        }
    }
}