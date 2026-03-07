using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Project.DTOs;
using Application.Features.Payrolls.Rules.Project.Handlers;
using Application.Features.Payrolls.Rules.Project.Queries;
using Application.Features.Payrolls.Rules.Project.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Project
{
    public class GetProjectRuleByIdEndpoint 
        : Endpoint<GetProjectRuleByIdQuery, ProjectRuleResponse>
    {
        private readonly GetProjectRuleByIdHandler _handler;

        public GetProjectRuleByIdEndpoint(
            GetProjectRuleByIdHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/project-payment-rules/{id:guid}");
            Roles("Administrator");
            Validator<GetProjectRuleByIdValidator>();

            Summary(s =>
            {
                s.Summary = "Get a project payment rule by its ID.";
                s.Description = "Retrieve the details of a specific project payment rule.";
                s.ExampleRequest = new
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000000")
                };
            });
        }

        public override async Task HandleAsync(
            GetProjectRuleByIdQuery req,
            CancellationToken ct)
        {
            req.Id = Route<Guid>("id");

            var result = await _handler.HandleAsync(req);

            if (result == null)
                await Send.NotFoundAsync(ct);
            else
                await Send.OkAsync(result, ct);
        }
    }
}
