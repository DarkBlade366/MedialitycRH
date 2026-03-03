using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Projects.DTOs;
using Application.Features.Projects.Handlers;
using Application.Features.Projects.Queries;
using Application.Features.Projects.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Projects
{
    public class GetMilestoneParticipationsPagedEndpoint : Endpoint<GetMilestoneParticipationsPagedQuery, PagedResponse<MilestoneParticipationResponse>>
    {
        private readonly GetMilestoneParticipationsPagedHandler _handler;

        public GetMilestoneParticipationsPagedEndpoint(GetMilestoneParticipationsPagedHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/milestone-participations");
            Roles("Administrator");
            Validator<GetMilestoneParticipationsPagedValidator>();
            Summary(s =>
            {
                s.Summary = "Get paged list of milestone participations";
                s.Description = "Retrieves a paged list of milestone participations with optional filters by milestone and active status.";
                s.ExampleRequest = new GetMilestoneParticipationsPagedQuery
                {
                    Page = 1,
                    PageSize = 10
                };
            });
        }

        public override async Task HandleAsync(GetMilestoneParticipationsPagedQuery req, CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);
            await Send.OkAsync(result, ct);
        }
    }
}