using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Projects.DTOs;
using Application.Features.Projects.Queries;
using Domain.Features.Projects.Interfaces;
using Application.Common.Interfaces;

namespace Application.Features.Projects.Handlers
{
    public class GetProjectByIdHandler
    {
        private readonly IProjectRepository _repository;
        private readonly ICacheService _cache;

        public GetProjectByIdHandler(IProjectRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<ProjectDto?> Handle(GetProjectByIdQuery query)
        {
            string cacheKey = $"project:redmine:{query.RedmineProjectId}";
            var cached = await _cache.GetAsync<ProjectDto>(cacheKey);
            if (cached != null)
                return cached;
            
            var project = await _repository.GetByRedmineIdAsync(query.RedmineProjectId);

            if (project == null) 
                return null;

            var response = new ProjectDto
            {
                Id = project.Id,
                RedmineProjectId = project.RedmineProjectId,
                Name = project.Name ?? string.Empty
            };

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));

            return response;
        }
    }
}