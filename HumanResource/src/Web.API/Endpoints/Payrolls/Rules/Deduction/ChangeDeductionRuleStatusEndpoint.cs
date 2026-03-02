using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Deduction.Commands;
using Application.Features.Payrolls.Rules.Deduction.Handlers;
using Application.Features.Payrolls.Rules.Deduction.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Deduction
{
    public class ChangeDeductionRuleStatusEndpoint : Endpoint<ChangeDeductionRuleStatusCommand>
    {
        private readonly ChangeDeductionRuleStatusHandler _handler;
    
        public ChangeDeductionRuleStatusEndpoint(ChangeDeductionRuleStatusHandler handler)
        {
            _handler = handler;
        }
    
        public override void Configure()
        {
            Put("/deduction-rules/{id:guid}/status");
            Roles("Administrator");
            Validator<ChangeDeductionRuleStatusValidator>();
            Summary (s =>
            {
                s.Summary = "Change the status of a deduction rule.";
                s.Description = "Activate or deactivate a deduction rule by its ID."; 
                s.ExampleRequest = new ChangeDeductionRuleStatusCommand
                {
                    IsActive = true
                };
            });
        }
    
        public override async Task HandleAsync(ChangeDeductionRuleStatusCommand req, CancellationToken ct)
        {
            req.Id = Route<Guid>("id");
    
            await _handler.HandleAsync(req);
            await Send.NoContentAsync(ct);
        }
    }
}