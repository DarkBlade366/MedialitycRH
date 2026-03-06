using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using Application.Features.TimeEntries.Commands;
using Application.Features.TimeEntries.Handlers;
using Application.Features.TimeEntries.Validations;
using Domain.Features.TimeEntries.Aggregates;
using Application.Features.TimeEntries.DTOs;

namespace Web.API.Endpoints.TimeEntries
{
    public class ApproveTimeEntryEndpoint : Endpoint<ApproveTimeEntryCommand, TimeEntryDto>
    {
        private readonly ApproveTimeEntryHandler _handler;

        public ApproveTimeEntryEndpoint(ApproveTimeEntryHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Put("/time-entries/approve");
            Roles("Administrator", "HumanResources", "ProjectManager");
            Validator<ApproveTimeEntryValidator>();
            Summary(s =>
            {
                s.Summary = "Approve a time entry.";
                s.Description = "Approves a time entry and sets the approved hours.";
                s.ExampleRequest = new ApproveTimeEntryCommand
                {
                    ApprovedHours = 5m
                };
            });
        }

        public override async Task HandleAsync(ApproveTimeEntryCommand req, CancellationToken ct)
        {
            var result = await _handler.Handle(req, ct);
            await Send.OkAsync(result, ct);
        }
    }
}