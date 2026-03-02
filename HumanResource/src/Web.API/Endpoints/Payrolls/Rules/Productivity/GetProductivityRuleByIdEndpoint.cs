using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Productivity.DTOs;
using Application.Features.Payrolls.Rules.Productivity.Handlers;
using Application.Features.Payrolls.Rules.Productivity.Queries;
using Application.Features.Payrolls.Rules.Productivity.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Productivity
{
    public class GetProductivityRuleByIdEndpoint 
        : Endpoint<GetProductivityRuleByIdQuery, ProductivityRuleResponse>
    {
        private readonly GetProductivityRuleByIdHandler _handler;

        public GetProductivityRuleByIdEndpoint(
            GetProductivityRuleByIdHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/productivity-rules/{id:guid}");
            Roles("Administrator");
            Validator<GetProductivityRuleByIdValidator>();
            Summary(s =>
            {
                s.Summary = "Get a productivity rule by ID.";
                s.Description = "Retrieve the details of a specific productivity rule.";
                s.ExampleRequest = new GetProductivityRuleByIdQuery
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000000")
                };
            });
        }

        public override async Task HandleAsync(
            GetProductivityRuleByIdQuery req,
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