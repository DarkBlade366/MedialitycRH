using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Projects.DTOs;
using Application.Features.Projects.Queries;
using Domain.Features.Projects.Interfaces;

namespace Application.Features.Projects.Handlers
{
    public class GetProjectByIdHandler
    {
        private readonly IProjectRepository _repository;

        public GetProjectByIdHandler(IProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProjectDto?> Handle(GetProjectByIdQuery query)
        {
            var project = await _repository.GetByRedmineIdAsync(query.RedmineProjectId);

            if (project == null) 
                return null;

            return new ProjectDto
            {
                Id = project.Id,
                RedmineProjectId = project.RedmineProjectId,
                Name = project.Name ?? string.Empty
            };
        }
    }
}