using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Vacation.Commands;
using Application.Features.Payrolls.Rules.Vacation.DTOs;
using Application.Features.Payrolls.Rules.Vacation.Handlers;
using Application.Features.Payrolls.Rules.Vacation.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Vacation
{
    public class CreateVacationRuleEndpoint : Endpoint<CreateVacationRuleCommand, VacationRuleResponse>
    {
        private readonly CreateVacationRuleHandler _handler;
    
        public CreateVacationRuleEndpoint(CreateVacationRuleHandler handler)
        {
            _handler = handler;
        }
    
        public override void Configure()
        {
            Post("/vacation-rules");
            Roles("Administrator");
            Validator<CreateVacationRuleValidator>();
            Summary(s =>
            {
                s.Summary = "Creates a new vacation rule.";
                s.Description = "Defines how many days are accrued per month and whether vacation is paid on use.";
                s.ExampleRequest = new CreateVacationRuleCommand
                {
                    AccrualRatePerMonth = 1.25m
                };
            });
        }
    
        public override async Task HandleAsync(CreateVacationRuleCommand req, CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result);
        }
    }
}