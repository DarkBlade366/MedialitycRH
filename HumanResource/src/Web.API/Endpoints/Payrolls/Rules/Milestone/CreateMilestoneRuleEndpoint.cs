using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Milestones.Commands;
using Application.Features.Payrolls.Rules.Milestones.DTOs;
using Application.Features.Payrolls.Rules.Milestones.Handlers;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Milestone
{
    public class CreateMilestoneRuleEndpoint : Endpoint<CreateMilestoneRuleCommand, MilestoneRuleResponse>
    {
        private readonly CreateMilestoneRuleHandler _handler;

        public CreateMilestoneRuleEndpoint(
            CreateMilestoneRuleHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Post("/milestone-rules");
            Roles("Administrator");
            Summary(s =>
            {
                s.Summary = "Creates a new milestone rule.";
                s.Description = "Creates a new milestone rule for a specific Redmine project.";
                s.ExampleRequest = new CreateMilestoneRuleCommand
                {
                    RedmineProjectId = 100,
                    MilestoneName = "Release 1.0",
                    BonusAmount = 500.00m
                };
            });
        }

        public override async Task HandleAsync(CreateMilestoneRuleCommand req,
            CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);

            await Send.OkAsync(result);
        }
    }
}