using System.Threading;
using System.Threading.Tasks;
using Application.Features.Redmine.DTOs;
using Application.Features.Redmine.Handlers;
using FastEndpoints;

namespace Web.API.Endpoints.Redmine
{
    public class GetRedmineTimeEntryActivitiesEndpoint : EndpointWithoutRequest<List<RedmineTimeEntryActivityDto>>
    {
        private readonly GetRedmineTimeEntryActivitiesHandler _handler;

        public GetRedmineTimeEntryActivitiesEndpoint(GetRedmineTimeEntryActivitiesHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/redmine/time-entry-activities");
            Roles("Administrator");
            Summary(s =>
            {
                s.Summary = "List Redmine time entry activities.";
                s.Description = "Returns id and name of each activity configured in Redmine. Use these ids when creating activity productivity weights.";
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var activities = await _handler.HandleAsync();
            await Send.OkAsync(activities, ct);
        }
    }
}
