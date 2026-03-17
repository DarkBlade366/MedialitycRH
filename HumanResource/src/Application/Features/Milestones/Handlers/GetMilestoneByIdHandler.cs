using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Milestones.DTOs;
using Application.Features.Milestones.Queries;
using Domain.Features.Projects.Interfaces;
using Application.Common.Interfaces; 

namespace Application.Features.Milestones.Handlers
{
    public class GetMilestoneByIdHandler
    {
        private readonly IProjectMilestoneRepository _repository;
        private readonly ICacheService _cache;

        public GetMilestoneByIdHandler(IProjectMilestoneRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<ProjectMilestoneDto?> Handle(GetMilestoneByIdQuery query)
        {
            string cacheKey = $"milestone:{query.Id}";
            var cached = await _cache.GetAsync<ProjectMilestoneDto>(cacheKey);
            if (cached != null)
                return cached;

            var milestone = await _repository.GetAllAsync();

            var item = milestone.FirstOrDefault(m => m.Id == query.Id);
            if (item == null) return null;

            var response = new ProjectMilestoneDto
            {
                Id = item.Id,
                RedmineProjectId = item.RedmineProjectId,
                Name = item.Name,
                Status = item.Status.ToString(),
                CompletedAt = item.CompletedAt
            };

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));

            return response;
        }
    }
}