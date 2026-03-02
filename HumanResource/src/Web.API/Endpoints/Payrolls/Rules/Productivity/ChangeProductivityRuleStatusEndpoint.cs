using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Productivity.Commands;
using Application.Features.Payrolls.Rules.Productivity.Handlers;
using Application.Features.Payrolls.Rules.Productivity.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Productivity
{
    public class ChangeProductivityRuleStatusEndpoint : Endpoint<ChangeProductivityRuleStatusCommand>
    {
        private readonly ChangeProductivityRuleStatusHandler _handler;
    
        public ChangeProductivityRuleStatusEndpoint(ChangeProductivityRuleStatusHandler handler)
        {
            _handler = handler;
        }
    
        public override void Configure()
        {
            Put("/productivity-rules/{id:guid}/status");
            Roles("Administrator");
            Validator<ChangeProductivityRuleStatusValidator>();
            Summary(s =>
            {
                s.Summary = "Changes the active status of a productivity rule.";
                s.Description = "Toggles the active status of a productivity rule by its ID.";
                s.ExampleRequest = new ChangeProductivityRuleStatusCommand
                {
                    IsActive = true
                };
            });
        }
    
        public override async Task HandleAsync(ChangeProductivityRuleStatusCommand req, CancellationToken ct)
        {
            req.Id = Route<Guid>("id");
    
            await _handler.HandleAsync(req);
            await Send.NoContentAsync(ct);
        }
    }
}