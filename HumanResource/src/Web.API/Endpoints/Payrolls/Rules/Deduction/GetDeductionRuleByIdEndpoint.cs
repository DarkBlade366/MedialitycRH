using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Deduction.DTOs;
using Application.Features.Payrolls.Rules.Deduction.Handlers;
using Application.Features.Payrolls.Rules.Deduction.Queries;
using Application.Features.Payrolls.Rules.Deduction.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Deduction
{
    public class GetDeductionRuleByIdEndpoint: Endpoint<GetDeductionRuleByIdQuery, DeductionRuleResponse>
    {
        private readonly GetDeductionRuleByIdHandler _handler;
    
        public GetDeductionRuleByIdEndpoint(
            GetDeductionRuleByIdHandler handler)
        {
            _handler = handler;
        }
    
        public override void Configure()
        {
            Get("/deduction-rules/{id:guid}");
            Roles("Administrator");
            Validator<GetDeductionRuleByIdValidator>();
            Summary(s =>
            {
                s.Summary = "Get a deduction rule by ID.";
                s.Description = "Retrieve the details of a specific deduction rule using its unique identifier.";
                s.ExampleRequest = new GetDeductionRuleByIdQuery
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000000")
                };
            });
        }
    
        public override async Task HandleAsync(GetDeductionRuleByIdQuery request, CancellationToken ct)
        {
            var result = await _handler.HandleAsync(request);
            
            if (result == null)
                await Send.NotFoundAsync(ct);
            else
                await Send.OkAsync(result, ct);
        }
    }
}
