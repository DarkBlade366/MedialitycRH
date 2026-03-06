using Application.Features.Payrolls.Rules.ActivityProductivityWeight.Commands;
using Application.Features.Payrolls.Rules.ActivityProductivityWeight.DTOs;
using Application.Features.Payrolls.Rules.ActivityProductivityWeight.Handlers;
using Application.Features.Payrolls.Rules.ActivityProductivityWeight.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.ActivityProductivityWeight
{
    public class CreateActivityProductivityWeightEndpoint
        : Endpoint<CreateActivityProductivityWeightCommand, ActivityProductivityWeightResponse>
    {
        private readonly CreateActivityProductivityWeightHandler _handler;

        public CreateActivityProductivityWeightEndpoint(CreateActivityProductivityWeightHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Post("/activity-productivity-weights");
            Roles("Administrator", "HumanResources");
            Validator<CreateActivityProductivityWeightValidator>();
            Summary(s =>
            {
                s.Summary = "Creates a new activity productivity weight.";
                s.Description = "Defines the weight (0-1) for a Redmine activity type. Used for weighted productivity calculation. Get the list of Redmine activity ids and names from GET /redmine/time-entry-activities so you use the correct id.";
                s.ExampleRequest = new CreateActivityProductivityWeightCommand
                {
                    RedmineActivityId = 9,
                    ActivityName = "Development",
                    Weight = 1.0m
                };
            });
        }

        public override async Task HandleAsync(CreateActivityProductivityWeightCommand req, CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result, ct);
        }
    }
}
