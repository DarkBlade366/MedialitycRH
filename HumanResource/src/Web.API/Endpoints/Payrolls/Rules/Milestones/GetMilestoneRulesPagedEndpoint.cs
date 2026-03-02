using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Rules.Milestones.DTOs;
using Application.Features.Payrolls.Rules.Milestones.Handlers;
using Application.Features.Payrolls.Rules.Milestones.Queries;
using Application.Features.Payrolls.Rules.Milestones.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Milestones
{
    public class GetMilestoneRulesPagedEndpoint 
        : Endpoint<GetMilestoneRulesPagedQuery, PagedResponse<MilestoneRuleResponse>>
    {
        private readonly GetMilestoneRulesPagedHandler _handler;

        public GetMilestoneRulesPagedEndpoint(GetMilestoneRulesPagedHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/milestone-rules");
            Roles("Administrator");
            Validator<GetMilestoneRulesPagedValidator>();
            Summary(s =>
            {
                s.Summary = "Get a paginated list of milestone rules.";
                s.Description = "Retrieve a paginated list of milestone rules with optional filtering and sorting.";
                s.ExampleRequest = new GetMilestoneRulesPagedQuery
                {
                    Page = 1,
                    PageSize = 10
                };
            });
        }

        public override async Task HandleAsync(
            GetMilestoneRulesPagedQuery req,
            CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result);
        }
    }
}
