using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Productivity.Commands;
using Application.Features.Payrolls.Rules.Productivity.DTOs;
using Application.Features.Payrolls.Rules.Productivity.Handlers;
using Application.Features.Payrolls.Rules.Productivity.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Productivity
{
    public class CreateProductivityRuleEndpoint 
        : Endpoint<CreateProductivityRuleCommand, ProductivityRuleResponse>
    {
        private readonly CreateProductivityRuleHandler _handler;

        public CreateProductivityRuleEndpoint(CreateProductivityRuleHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Post("/productivity-rules");
            Roles("Administrator");
            Validator<CreateProductivityRuleValidator>();
            Summary(s =>
            {
                s.Summary = "Creates a new productivity rule.";
                s.Description = "Creates a new productivity rule based on the provided details.";
                s.ExampleRequest = new CreateProductivityRuleCommand
                {
                    MinimumTarget = 100,
                    FullBonusTarget = 200,
                    BonusValue = 10,
                    BonusType = "Percentage",
                    MaxBonusCap = 50
                };
            });
        }

        public override async Task HandleAsync(CreateProductivityRuleCommand req, CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result, ct);
        }
    }
}