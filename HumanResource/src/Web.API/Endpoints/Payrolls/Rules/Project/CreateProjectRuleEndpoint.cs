using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Project.Commands;
using Application.Features.Payrolls.Rules.Project.DTOs;
using Application.Features.Payrolls.Rules.Project.Handlers;
using Application.Features.Payrolls.Rules.Project.Validations;
using FastEndpoints;
using FluentValidation;

namespace Web.API.Endpoints.Payrolls.Rules.Project
{
    public class CreateProjectRuleEndpoint : Endpoint<CreateProjectRuleCommand, ProjectRuleResponse>
    {
        private readonly CreateProjectRuleHandler _handler;

        public CreateProjectRuleEndpoint(CreateProjectRuleHandler handler)
        {
            _handler = handler;
        }
        public override void Configure()
        {
            Post("/project-payment-rules");
            Roles("Administrator", "HumanResources");
            Validator<CreateProjectRuleValidator>();
            Summary(s =>
            {
                s.Summary = "Creates a new project payment rule.";
                s.Description = "Creates a bonus rule that will be paid when the specified Redmine project is completed.";
                s.ExampleRequest = new CreateProjectRuleCommand
                {
                    RedmineProjectId = 100,
                    BonusAmount = 500.00m
                };
            });
        }

        public override async Task HandleAsync(CreateProjectRuleCommand req, CancellationToken ct)
        {

            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result, ct);
        }
    }
}
