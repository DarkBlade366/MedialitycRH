using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.BaseSalary.Commands;
using Application.Features.Payrolls.Rules.BaseSalary.Handlers;
using Application.Features.Payrolls.Rules.BaseSalary.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.BaseSalary
{
    public class ChangeBaseSalaryRuleStatusEndpoint : Endpoint<ChangeBaseSalaryRuleStatusCommand>
    {
        private readonly ChangeBaseSalaryRuleStatusHandler _handler;
    
        public ChangeBaseSalaryRuleStatusEndpoint(
            ChangeBaseSalaryRuleStatusHandler handler)
        {
            _handler = handler;
        }
    
        public override void Configure()
        {
            Put("/base-salary-rules/{id:guid}/status");
            Roles("Administrator");
            Validator<ChangeBaseSalaryRuleStatusValidator>();
            Summary(s =>
            {
                s.Summary = "Change the status of a base salary rule.";
                s.Description = "Activate or deactivate a base salary rule by its ID.";
                s.ExampleRequest = new ChangeBaseSalaryRuleStatusCommand
                {
                    IsActive = true
                };
            });
        }
    
        public override async Task HandleAsync(
            ChangeBaseSalaryRuleStatusCommand req,
            CancellationToken ct)
        {
            req.Id = Route<Guid>("id");
    
            await _handler.HandleAsync(req);
            await Send.NoContentAsync();
        }
    }
}