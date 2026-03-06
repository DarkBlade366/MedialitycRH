using Application.Common;
using Application.Features.Payrolls.Rules.ActivityProductivityWeight.DTOs;
using Application.Features.Payrolls.Rules.ActivityProductivityWeight.Handlers;
using Application.Features.Payrolls.Rules.ActivityProductivityWeight.Queries;
using Application.Features.Payrolls.Rules.ActivityProductivityWeight.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.ActivityProductivityWeight
{
    public class GetActivityProductivityWeightsPagedEndpoint
        : Endpoint<GetActivityProductivityWeightsPagedQuery, PagedResponse<ActivityProductivityWeightResponse>>
    {
        private readonly GetActivityProductivityWeightsPagedHandler _handler;

        public GetActivityProductivityWeightsPagedEndpoint(GetActivityProductivityWeightsPagedHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/activity-productivity-weights");
            Roles("Administrator", "HumanResources", "ProjectManager");
            Validator<GetActivityProductivityWeightsPagedValidator>();
            Summary(s =>
            {
                s.Summary = "Gets activity productivity weights (paginated).";
                s.Description = "Returns a paginated list of weights per Redmine activity type. Optional filter by IsActive.";
                s.ExampleRequest = new GetActivityProductivityWeightsPagedQuery
                {
                    Page = 1,
                    PageSize = 10
                };
            });
        }

        public override async Task HandleAsync(GetActivityProductivityWeightsPagedQuery req, CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result, ct);
        }
    }
}
