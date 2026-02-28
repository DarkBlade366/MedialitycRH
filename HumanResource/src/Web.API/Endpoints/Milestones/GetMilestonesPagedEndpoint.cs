using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Milestones.DTOs;
using Application.Features.Milestones.Handlers;
using Application.Features.Milestones.Queries;
using Application.Features.Milestones.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Milestones
{
    public class GetMilestonesPagedEndpoint: Endpoint<GetMilestonesPagedQuery, PagedResponse<ProjectMilestoneDto>>
    {
        private readonly GetMilestonesPagedHandler _handler;

        public GetMilestonesPagedEndpoint(GetMilestonesPagedHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/milestones");
            Roles("Administrator", "HumanResources", "ProjectManager");
            Validator<GetMilestonesPagedValidator>();
            Summary(s =>
            {
                s.Summary = "Get milestones paginated with filters.";
                s.Description = "Filters: ProjectId, Status, CompletedAt range, paginated.";
                s.ExampleRequest = new GetMilestonesPagedQuery
                {
                    Page = 1,
                    PageSize = 10
                };
            });
        }

        public override async Task HandleAsync(GetMilestonesPagedQuery req, CancellationToken ct)
        {
            var result = await _handler.Handle(req);
            await Send.OkAsync(result, ct);
        }
    }
}