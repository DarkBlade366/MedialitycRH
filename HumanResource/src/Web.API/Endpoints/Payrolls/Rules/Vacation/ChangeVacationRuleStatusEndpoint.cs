using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Vacation.Commands;
using Application.Features.Payrolls.Rules.Vacation.Handlers;
using Application.Features.Payrolls.Rules.Vacation.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Vacation
{
    public class ChangeVacationRuleStatusEndpoint : Endpoint<ChangeVacationRuleStatusCommand>
    {
        private readonly ChangeVacationRuleStatusHandler _handler;
    
        public ChangeVacationRuleStatusEndpoint(ChangeVacationRuleStatusHandler handler)
        {
            _handler = handler;
        }  
    
        public override void Configure()
        {
            Put("/vacation-rules/{id:guid}/status");
            Roles("Administrator");
            Validator<ChangeVacationRuleStatusValidator>();
            Summary(s =>
            {
                s.Summary = "Activates or deactivates a vacation rule.";
                s.Description = "Change the active status of a vacation rule.";
                s.ExampleRequest = new ChangeVacationRuleStatusCommand
                {
                    IsActive = true
                };
            });
        }
    
        public override async Task HandleAsync(ChangeVacationRuleStatusCommand req, CancellationToken ct)
        {
            req.Id = Route<Guid>("id");
            
            await _handler.HandleAsync(req);
            await Send.NoContentAsync(ct);
        }
    }
}