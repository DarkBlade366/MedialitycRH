using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Milestones.DTOs;
using Application.Features.Milestones.Handlers;
using Application.Features.Milestones.Queries;
using Application.Features.Milestones.Validations;
using FastEndpoints;

namespace Web.API.Endpoints.Milestones
{
    public class GetMilestoneByIdEndpoint: Endpoint<GetMilestoneByIdQuery, ProjectMilestoneDto>
    {
        private readonly GetMilestoneByIdHandler _handler;

        public GetMilestoneByIdEndpoint(GetMilestoneByIdHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/milestones/{Id}");
            Roles("Administrator", "HumanResources", "ProjectManager");
            Validator<GetMilestoneByIdValidator>();
            Summary(s =>
            {
                s.Summary = "Get milestone by Id.";
                s.Description = "Returns a single milestone by its Id.";
                s.ExampleRequest = new GetMilestoneByIdQuery
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000000")
                };
            });
        }

        public override async Task HandleAsync(GetMilestoneByIdQuery req, CancellationToken ct)
        {
            var result = await _handler.Handle(req);
            
            if (result == null)
                await Send.NotFoundAsync(ct);
            else
                await Send.OkAsync(result, ct);
        }
    }
}