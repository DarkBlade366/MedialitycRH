using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Project.Commands;
using Application.Features.Payrolls.Rules.Project.Handlers;
using Application.Features.Payrolls.Rules.Project.Validations;
using FastEndpoints;
using FluentValidation;

namespace Web.API.Endpoints.Payrolls.Rules.Project
{
    public class ChangeProjectRuleStatusEndpoint : Endpoint<ChangeProjectRuleStatusCommand>
    {
        private readonly ChangeProjectRuleStatusHandler _handler;

        public ChangeProjectRuleStatusEndpoint(
            ChangeProjectRuleStatusHandler handler)
        {
            _handler = handler;
        }
        public override void Configure()
        {
            Put("/project-payment-rules/{id:guid}/status");
            Roles("Administrator", "HumanResources");
            Validator<ChangeProjectRuleStatusValidator>();
            Summary(s =>
            {
                s.Summary = "Change the status of a project payment rule.";
                s.Description = "Activate or deactivate a project payment rule.";
                s.ExampleRequest = new ChangeProjectRuleStatusCommand
                {
                    IsActive = true
                };
            });
        }

        public override async Task HandleAsync(ChangeProjectRuleStatusCommand req, CancellationToken ct)
        {
            req.Id = Route<Guid>("id");

            await _handler.HandleAsync(req);
            await Send.NoContentAsync();
        }
    }
}
