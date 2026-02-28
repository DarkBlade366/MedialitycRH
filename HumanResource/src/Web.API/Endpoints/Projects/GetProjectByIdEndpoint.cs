using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Projects.Queries;
using Application.Features.Projects.DTOs;
using Application.Features.Projects.Handlers;
using Application.Features.Projects.Validations;
using FastEndpoints;


namespace Web.API.Endpoints.Projects
{
    public class GetProjectByIdEndpoint : Endpoint<GetProjectByIdQuery, ProjectDto>
    {
        private readonly GetProjectByIdHandler _handler;

        public GetProjectByIdEndpoint(GetProjectByIdHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/projects/{RedmineProjectId}");
            Roles("Administrator", "HumanResources", "ProjectManager");
            Validator<GetProjectByIdValidator>();
            Summary(s =>
            {
                s.Summary = "Get project by Redmine ID.";
                s.Description = "Returns a single project based on RedmineProjectId.";
                s.ExampleRequest = new GetProjectByIdQuery { 
                    RedmineProjectId = 100 
                };
            });
        }

        public override async Task HandleAsync(GetProjectByIdQuery req, CancellationToken ct)
        {
            var result = await _handler.Handle(req);
            
            if (result == null)
                await Send.NotFoundAsync(ct);
            else
                await Send.OkAsync(result, ct);
        }
    }
}