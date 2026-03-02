using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Deduction.Commands;
using Application.Features.Payrolls.Rules.Deduction.DTOs;
using Application.Features.Payrolls.Rules.Deduction.Handlers;
using Application.Features.Payrolls.Rules.Deduction.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Deduction
{
    public class CreateDeductionRuleEndpoint : Endpoint<CreateDeductionRuleCommand, DeductionRuleResponse>
    {
        private readonly CreateDeductionRuleHandler _handler;
    
        public CreateDeductionRuleEndpoint(CreateDeductionRuleHandler handler)
        {
            _handler = handler;
        }
    
        public override void Configure()
        {
            Post("/deduction-rules");
            Roles("Administrator");
            Validator<CreateDeductionRuleValidator>();
            Summary(s =>
            {
                s.Summary = "Create a new deduction rule.";
                s.Description = "Add a new deduction rule to the payroll system.";
                s.ExampleRequest = new CreateDeductionRuleCommand
                {
                    Description = "Health Insurance Deduction",
                    Percentage = 5.5m,
                    Type = "BasicSalary"
                };
            });
        }
    
        public override async Task HandleAsync(CreateDeductionRuleCommand req, CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result, ct);
        }
    }
}