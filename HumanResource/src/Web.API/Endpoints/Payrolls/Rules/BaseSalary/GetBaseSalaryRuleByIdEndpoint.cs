using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.BaseSalary.DTOs;
using Application.Features.Payrolls.Rules.BaseSalary.Handlers;
using Application.Features.Payrolls.Rules.BaseSalary.Queries;
using Application.Features.Payrolls.Rules.BaseSalary.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.BaseSalary
{
    public class GetBaseSalaryRuleByIdEndpoint : Endpoint<GetBaseSalaryRuleByIdQuery, BaseSalaryRuleResponse>
    {
        private readonly GetBaseSalaryRuleByIdHandler _handler;

        public GetBaseSalaryRuleByIdEndpoint(
            GetBaseSalaryRuleByIdHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/base-salary-rules/{id:guid}");
            Roles("Administrator");
            Validator<GetBaseSalaryRuleByIdValidator>();
            Summary(s =>
            {
                s.Summary = "Get a base salary rule by ID.";
                s.Description = "Retrieve the details of a specific base salary rule using its unique identifier.";
                s.ExampleRequest = new GetBaseSalaryRuleByIdQuery
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000000")
                };
            });
        }

        public override async Task HandleAsync(
            GetBaseSalaryRuleByIdQuery req,
            CancellationToken ct)
        {
            req.Id = Route<Guid>("id");

            var result = await _handler.HandleAsync(req);

            if (result is null)
                await Send.NotFoundAsync();
            else
                await Send.OkAsync(result);
        }
    }
}