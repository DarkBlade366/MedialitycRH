using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Overtime.Commands;
using Application.Features.Payrolls.Rules.Overtime.Handlers;
using Application.Features.Payrolls.Rules.Overtime.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Overtime
    {
    public class ChangeOvertimeRuleStatusEndpoint : Endpoint<ChangeOvertimeRuleStatusCommand>
    {
        private readonly ChangeOvertimeRuleStatusHandler _handler;

        public ChangeOvertimeRuleStatusEndpoint(ChangeOvertimeRuleStatusHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Put("/overtime-rules/{id:guid}/status");
            Roles("Administrator");
            Validator<ChangeOvertimeRuleStatusValidator>();
            Summary(s =>
            {
                s.Summary = "Change the status of an overtime rule.";
                s.Description = "Activate or deactivate an overtime rule by changing its status.";
                s.ExampleRequest = new ChangeOvertimeRuleStatusCommand
                {
                    IsActive = true
                };
            });
        }

        public override async Task HandleAsync(
            ChangeOvertimeRuleStatusCommand req,
            CancellationToken ct)
        {
            req.Id = Route<Guid>("id");

            await _handler.HandleAsync(req);
            await Send.NoContentAsync();
        }
        
    }
}