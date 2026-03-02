using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Aguinaldo.Commands;
using Application.Features.Payrolls.Rules.Aguinaldo.Handlers;
using Application.Features.Payrolls.Rules.Aguinaldo.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Aguinaldo
{
    public class ChangeAguinaldoRuleStatusEndpoint : Endpoint<ChangeAguinaldoRuleStatusCommand>
    {
        private readonly ChangeAguinaldoRuleStatusHandler _handler;
        public ChangeAguinaldoRuleStatusEndpoint(ChangeAguinaldoRuleStatusHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Put("/aguinaldo-rules/{id:guid}/status");
            Roles("Administrator");
            Validator<ChangeAguinaldoRuleStatusValidator>();
            Summary(s =>
            {
                s.Summary = "Change the status of an aguinaldo rule.";
                s.Description = "Activate or deactivate an aguinaldo rule by changing its status.";
                s.ExampleRequest = new ChangeAguinaldoRuleStatusCommand
                {
                    IsActive = true
                };  
            });
        }

        public override async Task HandleAsync(ChangeAguinaldoRuleStatusCommand req, CancellationToken ct)
        {
            var id = Route<Guid>("id");

            await _handler.HandleAsync(req);
            await Send.NoContentAsync();
        }
    }
}
