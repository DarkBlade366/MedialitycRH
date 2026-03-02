using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Rules.Deduction.DTOs;
using Application.Features.Payrolls.Rules.Deduction.Handlers;
using Application.Features.Payrolls.Rules.Deduction.Queries;
using Application.Features.Payrolls.Rules.Deduction.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Deduction
{
    public class GetDeductionRulesPagedEndpoint 
        : Endpoint<GetDeductionRulesPagedQuery, PagedResponse<DeductionRuleResponse>>
    {
        private readonly GetDeductionRulesPagedHandler _handler;

        public GetDeductionRulesPagedEndpoint(GetDeductionRulesPagedHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/deduction-rules");
            Roles("Administrator");
            Validator<GetDeductionRulesPagedValidator>();
            Summary(s =>
            {
                s.Summary = "Get a paginated list of deduction rules.";
                s.Description = "Retrieve a paginated list of deduction rules with optional filtering and sorting.";
                s.ExampleRequest = new GetDeductionRulesPagedQuery
                {
                    Page = 1,
                    PageSize = 10
                };
            });
        }

        public override async Task HandleAsync(
            GetDeductionRulesPagedQuery req,
            CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result);
        }
    }
}
