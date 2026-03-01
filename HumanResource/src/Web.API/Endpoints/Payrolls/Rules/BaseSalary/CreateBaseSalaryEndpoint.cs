using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.BaseSalary.Commands;
using Application.Features.Payrolls.Rules.BaseSalary.DTOs;
using Application.Features.Payrolls.Rules.BaseSalary.Handlers;
using Application.Features.Payrolls.Rules.BaseSalary.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.BaseSalary
{
    public class CreateBaseSalaryEndpoint : Endpoint<CreateBaseSalaryRuleCommand, BaseSalaryRuleResponse>
    {
        private readonly CreateBaseSalaryRuleHandler _handler;

        public CreateBaseSalaryEndpoint(CreateBaseSalaryRuleHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Post("/base-salary-rules");
            Roles("Administrator");
            Validator<CreateBaseSalaryRuleValidator>();
            Summary(s =>
            {
                s.Summary = "Create a new base salary rule.";
                s.Description = "Add a new base salary rule to the payroll system.";
                s.ExampleRequest = new CreateBaseSalaryRuleCommand
                {
                    Amount = 3000.00m,
                    Role = "Employee"
                };
            });
        }

        public override async Task HandleAsync(
            CreateBaseSalaryRuleCommand req,
            CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result);
        }
    }
}