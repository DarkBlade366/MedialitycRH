using Application.Features.Payrolls.Rules.ActivityProductivityWeight.Commands;
using Application.Features.Payrolls.Rules.ActivityProductivityWeight.Handlers;
using FastEndpoints;

namespace Web.API.Endpoints.Payrolls.Rules.ActivityProductivityWeight
{
    public class ChangeActivityProductivityWeightStatusEndpoint : Endpoint<ChangeActivityProductivityWeightStatusCommand>
    {
        private readonly ChangeActivityProductivityWeightStatusHandler _handler;

        public ChangeActivityProductivityWeightStatusEndpoint(ChangeActivityProductivityWeightStatusHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Put("/activity-productivity-weights/{id:guid}/status");
            Roles("Administrator", "HumanResources");
            Summary(s =>
            {
                s.Summary = "Activates or deactivates an activity productivity weight.";
                s.Description = "Changes the active status of an activity productivity weight.";
                s.ExampleRequest = new ChangeActivityProductivityWeightStatusCommand
                {
                    IsActive = true
                };
            });
        }

        public override async Task HandleAsync(ChangeActivityProductivityWeightStatusCommand req, CancellationToken ct)
        {
            req.Id = Route<Guid>("id");
            await _handler.HandleAsync(req);
            await Send.NoContentAsync(ct);
        }
    }
}
