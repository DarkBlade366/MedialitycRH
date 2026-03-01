using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Milestones.DTOs;
using Application.Features.Payrolls.Rules.Milestones.Handlers;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Milestone
{
    public class GetMilestoneRuleByIdEndpoint: EndpointWithoutRequest<MilestoneRuleResponse>
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
    
        public override async Task HandleAsync(CancellationToken ct)
        {
            var id = Route<Guid>("id");
    
            var result = await _handler.HandleAsync(id);
    
            if (result is null)
            {
                await Send.NotFoundAsync();
                return;
            }
    
            await Send.OkAsync(result);
        }
    }
}