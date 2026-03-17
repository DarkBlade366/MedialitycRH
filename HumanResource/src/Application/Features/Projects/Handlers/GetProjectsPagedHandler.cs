using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Projects.DTOs;
using Application.Features.Projects.Queries;
using Domain.Features.Projects.Interfaces;
using Domain.Features.Projects.Aggregates;
using Application.Common.Interfaces;

namespace Application.Features.Projects.Handlers
{
    public class GetProjectsPagedHandler
    {
        private readonly IProjectRepository _repository;
        private readonly ICacheService _cache;

        public GetProjectsPagedHandler(IProjectRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<PagedResponse<ProjectDto>> Handle(GetProjectsPagedQuery query)
        {
            string cacheKey = "projects:all";

            var allProjects = await _cache.GetAsync<List<Project>>(cacheKey);
            if (allProjects == null)
            {
                allProjects = await _repository.GetAllAsync();
                await _cache.SetAsync(cacheKey, allProjects, TimeSpan.FromMinutes(10));
            }
            var totalItems = allProjects.Count; 

            var paged = allProjects
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    RedmineProjectId = p.RedmineProjectId,
                    Name = p.Name ?? string.Empty
                }).ToList();

            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

            return new PagedResponse<ProjectDto>
            {
                Items = paged,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }
    }
}