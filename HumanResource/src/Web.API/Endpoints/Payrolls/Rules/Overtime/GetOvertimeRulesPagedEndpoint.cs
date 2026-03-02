using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Rules.Overtime.DTOs;
using Application.Features.Payrolls.Rules.Overtime.Handlers;
using Application.Features.Payrolls.Rules.Overtime.Queries;
using Application.Features.Payrolls.Rules.Overtime.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Overtime
{
    public class GetOvertimeRulesPagedEndpoint 
        : Endpoint<GetOvertimeRulesPagedQuery, PagedResponse<OvertimeRuleResponse>>
    {
        private readonly GetOvertimeRulesPagedHandler _handler;

        public GetOvertimeRulesPagedEndpoint(GetOvertimeRulesPagedHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/overtime-rules");
            Roles("Administrator");
            Validator<GetOvertimeRulesPagedValidator>();
            Summary(s =>
            {
                s.Summary = "Get a paginated list of overtime rules.";
                s.Description = "Retrieve a paginated list of overtime rules with optional filtering and sorting.";
                s.ExampleRequest = new GetOvertimeRulesPagedQuery
                {
                    Page = 1,
                    PageSize = 10
                };
            });
        }

        public override async Task HandleAsync(
            GetOvertimeRulesPagedQuery req,
            CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result);
        }
    }
}