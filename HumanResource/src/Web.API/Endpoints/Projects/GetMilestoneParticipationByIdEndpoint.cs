using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastEndpoints;
using Application.Features.Projects.Queries;
using Application.Features.Projects.DTOs;
using Application.Features.Projects.Handlers;
using Application.Features.Projects.Validations;

namespace Web.API.Endpoints.Projects
{
    public class GetMilestoneParticipationByIdEndpoint : Endpoint<GetMilestoneParticipationByIdQuery, MilestoneParticipationResponse>
    {
        private readonly GetMilestoneParticipationByIdHandler _handler;

        public GetMilestoneParticipationByIdEndpoint(GetMilestoneParticipationByIdHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/milestone-participations/{Id}");
            Roles("Administrator");
            Validator<GetMilestoneParticipationByIdValidator>();
            Summary(s =>
            {
                s.Summary = "Gets a milestone participation by its Id.";
                s.Description = "Returns the participation details of an employee for a specific milestone.";
                s.ExampleRequest = new GetMilestoneParticipationByIdQuery
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000000")
                };
            });
        }

        public override async Task HandleAsync(GetMilestoneParticipationByIdQuery req, CancellationToken ct)
        {
            var result = await _handler.HandleAsync(req);

            if (result == null)
                await Send.NotFoundAsync(ct);
            else
                await Send.OkAsync(result, ct);
        }
    }
}