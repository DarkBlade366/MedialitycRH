using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Projects.DTOs;
using Application.Features.Projects.Queries;
using Domain.Features.Projects.Interfaces;

namespace Application.Features.Projects.Handlers
{
    public class GetProjectsPagedHandler
    {
        private readonly IProjectRepository _repository;

        public GetProjectsPagedHandler(IProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<ProjectDto>> Handle(GetProjectsPagedQuery query)
        {
            var allProjects = await _repository.GetAllAsync();
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