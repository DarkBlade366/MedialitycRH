using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Rules.BaseSalary.DTOs;
using Application.Features.Payrolls.Rules.BaseSalary.Handlers;
using Application.Features.Payrolls.Rules.BaseSalary.Queries;
using Application.Features.Payrolls.Rules.BaseSalary.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.BaseSalary
{
    public class GetBaseSalaryRulesPagedEndpoint 
        : Endpoint<GetBaseSalaryRulesPagedQuery, PagedResponse<BaseSalaryRuleResponse>>
    {
        private readonly GetBaseSalaryRulesPagedHandler _handler;

        public GetBaseSalaryRulesPagedEndpoint(
            GetBaseSalaryRulesPagedHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/base-salary-rules");
            Roles("Administrator");
            Validator<GetBaseSalaryRulesPagedValidator>();
            Summary(s =>
            {
                s.Summary = "Get a paginated list of base salary rules.";
                s.Description = "Retrieve a paginated list of base salary rules with optional filtering and sorting.";
                s.ExampleRequest = new GetBaseSalaryRulesPagedQuery
                {
                    Page = 1,
                    PageSize = 10
                };
            });
        }

        public override async Task HandleAsync(
            GetBaseSalaryRulesPagedQuery req,
            CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result);
        }
    }
}