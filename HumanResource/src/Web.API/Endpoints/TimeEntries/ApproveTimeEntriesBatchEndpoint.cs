using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using Application.Features.TimeEntries.Commands;
using Application.Features.TimeEntries.DTOs;
using Application.Features.TimeEntries.Handlers;
using Application.Features.TimeEntries.Validations;

namespace Web.API.Endpoints.TimeEntries
{
    public class ApproveTimeEntriesBatchEndpoint 
        : Endpoint<ApproveTimeEntriesBatchCommand, List<TimeEntryBatchResultDto>>
    {
        private readonly ApproveTimeEntriesBatchHandler _handler;

        public ApproveTimeEntriesBatchEndpoint(ApproveTimeEntriesBatchHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Put("/time-entries/approve-batch");
            Roles("Administrator", "HumanResources", "ProjectManager");
            Validator<ApproveTimeEntriesBatchValidator>();
            Summary(s =>
            {
                s.Summary = "Approve multiple time entries in batch.";
                s.Description = "Approves a list of time entries. Already reviewed entries are skipped.";
                s.ExampleRequest = new ApproveTimeEntriesBatchCommand
                {
                    Items = new List<ApproveTimeEntriesBatchCommand.ApproveTimeEntryItem>
                    {
                        new()
                        {
                            TimeEntryId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                            ApprovedHours = 2
                        },
                        new()
                        {
                            TimeEntryId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                            ApprovedHours = 0
                        }
                    }
                };
            });
        }

        public override async Task HandleAsync(
            ApproveTimeEntriesBatchCommand req, 
            CancellationToken ct)
        {
            var result = await _handler.Handle(req, ct);

            if (result.Count == 0)
                await Send.NoContentAsync(ct);
            else
                await Send.OkAsync(result, ct);
        }
    }
}