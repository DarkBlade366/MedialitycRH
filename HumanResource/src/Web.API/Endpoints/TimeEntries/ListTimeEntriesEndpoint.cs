using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.TimeEntries.DTOs;
using Application.Features.TimeEntries.Handlers;
using Application.Features.TimeEntries.Queries;
using Application.Features.TimeEntries.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.TimeEntries
{
    public class ListTimeEntriesEndpoint : Endpoint<ListTimeEntriesQuery>
    {
        private readonly ListTimeEntriesHandler _handler;

        public ListTimeEntriesEndpoint(ListTimeEntriesHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/time-entries");
            Roles("Administrator", "HumanResources", "ProjectManager");
            Validator<ListTimeEntriesQueryValidator>();
            Summary(s =>
            {
                s.Summary = "List time entries by employee and date range.";
                s.Description = "Returns all time entries for a given employee between the specified date range (UTC required).";
                s.ExampleRequest = new ListTimeEntriesQuery
                {
                    From = DateTime.UtcNow.AddDays(-30),
                    To = DateTime.UtcNow
                };
            });
        }

        public override async Task HandleAsync(ListTimeEntriesQuery req, CancellationToken ct)
        {
            var result = await _handler.Handle(req);
            await Send.OkAsync(result, ct);
        }
    }
}