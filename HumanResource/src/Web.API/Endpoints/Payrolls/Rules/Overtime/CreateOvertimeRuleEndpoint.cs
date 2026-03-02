using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Overtime.Commands;
using Application.Features.Payrolls.Rules.Overtime.DTOs;
using Application.Features.Payrolls.Rules.Overtime.Handlers;
using Application.Features.Payrolls.Rules.Overtime.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Overtime
{
    public class CreateOvertimeRuleEndpoint : Endpoint<CreateOvertimeRuleCommand, OvertimeRuleResponse>
    {
        private readonly CreateOvertimeRuleHandler _handler;
    
        public CreateOvertimeRuleEndpoint(CreateOvertimeRuleHandler handler)
        {
            _handler = handler;
        }
    
        public override void Configure()
        {
            Post("/overtime-rules");
            Roles("Administrator");
            Validator<CreateOvertimeRuleValidator>();
            Summary(s =>
            {
                s.Summary = "Creates a new overtime rule.";
                s.Description = "Creates a new overtime rule with the specified standard hours and multiplier.";
                s.ExampleRequest = new CreateOvertimeRuleCommand
                {
                    StandardHoursPerPeriod = 160,
                    OvertimeMultiplier = 1.5m
                };
            });
        }
    
        public override async Task HandleAsync(
            CreateOvertimeRuleCommand req,
            CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result);
        }
    }
}