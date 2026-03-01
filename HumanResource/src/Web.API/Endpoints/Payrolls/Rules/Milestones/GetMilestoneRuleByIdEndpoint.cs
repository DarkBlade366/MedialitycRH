using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Milestones.Validations;
using Application.Features.Payrolls.Rules.Milestones.DTOs;
using Application.Features.Payrolls.Rules.Milestones.Handlers;
using Application.Features.Payrolls.Rules.Milestones.Queries;
using Application.Features.Payrolls.Rules.Milestones.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Milestones
{
    public class GetMilestoneRuleByIdEndpoint: Endpoint<GetMilestoneRuleByIdQuery, MilestoneRuleResponse>
    {
        private readonly GetMilestoneRuleByIdHandler _handler;
    
        public GetMilestoneRuleByIdEndpoint(
            GetMilestoneRuleByIdHandler handler)
        {
            _handler = handler;
        }
    
        public override void Configure()
        {
            Get("/milestone-rules/{id:guid}");
            Roles("Administrator");
            Validator<GetMilestoneRuleByIdValidator>();
            Summary(s =>
            {
                s.Summary = "Get a milestone rule by its ID.";
                s.Description = "Retrieve the details of a specific milestone rule using its unique identifier.";
                s.ExampleRequest = new
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000000")
                };
            });
        }
    
        public override async Task HandleAsync(GetMilestoneRuleByIdQuery request, CancellationToken ct)
        {
            var result = await _handler.HandleAsync(request);
            
            if (result == null)
                await Send.NotFoundAsync(ct);
            else
                await Send.OkAsync(result, ct);
        }
    }
}
