using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Rules.Project.DTOs;
using Application.Features.Payrolls.Rules.Project.Handlers;
using Application.Features.Payrolls.Rules.Project.Queries;
using Application.Features.Payrolls.Rules.Project.Validations;
using FastEndpoints;
using FluentValidation;

namespace Web.API.Endpoints.Payrolls.Rules.Project
{
    public class GetProjectRulesPagedEndpoint : Endpoint<GetProjectRulesPagedQuery, PagedResponse<ProjectRuleResponse>>
    {
        private readonly GetProjectRulesPagedHandler _handler;

        public GetProjectRulesPagedEndpoint(
            GetProjectRulesPagedHandler handler)
        {
            _handler = handler;
        }
        public override void Configure()
        {
            Get("/project-payment-rules");
            Roles("Administrator", "HumanResources", "ProjectManager");
            Validator<GetProjectRulesPagedValidator>();
            Summary(s =>
            {
                s.Summary = "Get a paginated list of project payment rules.";
                s.Description = "Retrieve a paginated list of project payment rules with optional filtering.";

                s.ExampleRequest = new GetProjectRulesPagedQuery
                {
                    Page = 1,
                    PageSize = 10
                };
            });
        }

        public override async Task HandleAsync(GetProjectRulesPagedQuery req, CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result, ct);
        }
    }
}
