using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Aguinaldo.Commands;
using Application.Features.Payrolls.Rules.Aguinaldo.DTOs;
using Application.Features.Payrolls.Rules.Aguinaldo.Handlers;
using Application.Features.Payrolls.Rules.Aguinaldo.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Aguinaldos
{
    public class CreateAguinaldoEndpoint : Endpoint<CreateAguinaldoRuleCommand, AguinaldoRuleResponse>
    {
        private readonly CreateAguinaldoRuleHandler _handler;
        public CreateAguinaldoEndpoint(CreateAguinaldoRuleHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Post("/aguinaldo-rules");
            Roles("Administrator");
            Validator<CreateAguinaldoRuleValidator>();
            Summary(s =>
            {
                s.Summary = "Creates a new aguinaldo rule.";
                s.Description = "Creates a new aguinaldo rule. The rule will be applied to calculate the aguinaldo payment for employees based on the specified accrual percentage.";
                s.ExampleRequest = new CreateAguinaldoRuleCommand
                {
                    PayMonth = 1,
                    MonthlyAccrualPercentage = 0.083m
                };
            });
        }

        public override async Task HandleAsync(CreateAguinaldoRuleCommand req,
            CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result);
        }
    }
}