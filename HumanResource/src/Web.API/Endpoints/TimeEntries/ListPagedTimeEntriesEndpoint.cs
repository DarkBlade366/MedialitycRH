using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Application.Common;
using Application.Features.TimeEntries.DTOs;
using Application.Features.TimeEntries.Handlers;
using Application.Features.TimeEntries.Queries;
using Application.Features.TimeEntries.Validations;

namespace Web.API.Endpoints.TimeEntries
{
    public class ListPagedTimeEntriesEndpoint : Endpoint<ListPagedTimeEntriesQuery, PagedResponse<TimeEntryDto>>
    {
        private readonly ListPagedTimeEntriesHandler _handler;

        public ListPagedTimeEntriesEndpoint(ListPagedTimeEntriesHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/time-entries/paged");
            Roles("Administrator", "HumanResources", "ProjectManager");
            Validator<ListPagedTimeEntriesValidator>();
            Summary(s =>
            {
                s.Summary = "List paged time entries by employee and date range.";
                s.Description = "Returns paginated time entries for a given employee within a date range (UTC required).";
                s.ExampleRequest = new ListPagedTimeEntriesQuery
                {
                    Page = 1,
                    PageSize = 10
                };
            });
        }

        public override async Task HandleAsync(ListPagedTimeEntriesQuery req, CancellationToken ct)
        {
            var result = await _handler.Handle(req);

            if (result.Items.Count == 0)
            {
                await Send.NoContentAsync(ct);
                return;
            }

            await Send.OkAsync(result, ct);
        }
    }
}
