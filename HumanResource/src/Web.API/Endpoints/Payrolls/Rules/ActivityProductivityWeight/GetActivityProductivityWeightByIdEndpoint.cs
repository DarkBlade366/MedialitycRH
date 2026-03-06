using Application.Features.Payrolls.Rules.ActivityProductivityWeight.DTOs;
using Application.Features.Payrolls.Rules.ActivityProductivityWeight.Handlers;
using Application.Features.Payrolls.Rules.ActivityProductivityWeight.Queries;
using Application.Features.Payrolls.Rules.ActivityProductivityWeight.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.ActivityProductivityWeight
{
    public class GetActivityProductivityWeightByIdEndpoint
        : Endpoint<GetActivityProductivityWeightByIdQuery, ActivityProductivityWeightResponse>
    {
        private readonly GetActivityProductivityWeightByIdHandler _handler;

        public GetActivityProductivityWeightByIdEndpoint(GetActivityProductivityWeightByIdHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/activity-productivity-weights/{id:guid}");
            Roles("Administrator", "HumanResources", "ProjectManager");
            Validator<GetActivityProductivityWeightByIdValidator>();
            Summary(s =>
            {
                s.Summary = "Gets an activity productivity weight by ID.";
                s.Description = "Gets the productivity weight for a specific activity.";
                s.ExampleRequest = new ActivityProductivityWeightResponse
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000000")
                };
            });
        }

        public override async Task HandleAsync(GetActivityProductivityWeightByIdQuery req, CancellationToken ct)
        {
            req.Id = Route<Guid>("id");
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result, ct);
        }
    }
}
