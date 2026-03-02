using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Rules.Overtime.DTOs;
using Application.Features.Payrolls.Rules.Overtime.Handlers;
using Application.Features.Payrolls.Rules.Overtime.Queries;
using Application.Features.Payrolls.Rules.Overtime.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.Overtime
{
    public class GetOvertimeRuleByIdEndpoint : Endpoint<GetOvertimeRuleByIdQuery, OvertimeRuleResponse>
    {
        private readonly GetOvertimeRuleByIdHandler _handler;
    
        public GetOvertimeRuleByIdEndpoint(GetOvertimeRuleByIdHandler handler)
        {
            _handler = handler;
        }
    
        public override void Configure()
        {
            Get("/overtime-rules/{id:guid}");
            Roles("Administrator");
            Validator<GetOvertimeRuleByIdValidator>();
            Summary(s =>
            {
                s.Summary = "Get an overtime rule by its ID.";
                s.Description = "Retrieve the details of a specific overtime rule using its unique identifier.";
                s.ExampleRequest = new GetOvertimeRuleByIdQuery
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000000")
                };
            });
        }
    
        public override async Task HandleAsync(
            GetOvertimeRuleByIdQuery req,
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