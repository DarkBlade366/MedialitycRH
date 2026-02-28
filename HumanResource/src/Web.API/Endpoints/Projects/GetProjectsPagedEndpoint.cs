using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Projects.Queries;
using Application.Features.Projects.DTOs;
using Application.Features.Projects.Handlers;
using Application.Features.Projects.Validations;
using Application.Common;
using FastEndpoints;

namespace Web.API.Endpoints.Projects
{
    public class GetProjectsPagedEndpoint : Endpoint<GetProjectsPagedQuery, PagedResponse<ProjectDto>>
    {
        private readonly GetProjectsPagedHandler _handler;

        public GetProjectsPagedEndpoint(GetProjectsPagedHandler handler)
        {
            _handler = handler;
        }

        public override void Configure()
        {
            Get("/projects");
            Roles("Administrator", "HumanResources", "ProjectManager");
            Validator<GetProjectsPagedValidator>();
            Summary(s =>
            {
                s.Summary = "Get paged projects.";
                s.Description = "Returns projects paginated (Redmine projects).";
                s.ExampleRequest = new GetProjectsPagedQuery { 
                    Page = 1, 
                    PageSize = 10 
                };
            });
        }

        public override async Task HandleAsync(GetProjectsPagedQuery req, CancellationToken ct)
        {
            var result = await _handler.Handle(req);
            await Send.OkAsync(result, ct);
        }
    }
}