using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Milestones.DTOs;
using Application.Features.Milestones.Queries;
using Domain.Features.Projects.Enums;
using Domain.Features.Projects.Interfaces;
using Application.Common.Interfaces;
using Domain.Features.Projects.Aggregates;

namespace Application.Features.Milestones.Handlers
{
    public class GetMilestonesPagedHandler
    {
        private readonly IProjectMilestoneRepository _repository;
        private readonly ICacheService _cache;

        public GetMilestonesPagedHandler(IProjectMilestoneRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<PagedResponse<ProjectMilestoneDto>> Handle(GetMilestonesPagedQuery query)
        {
            string cacheKey = "milestones:all";
            var allMilestones = await _cache.GetAsync<List<ProjectMilestone>>(cacheKey);
            if (allMilestones == null)
            {
                allMilestones = await _repository.GetAllAsync();
                await _cache.SetAsync(cacheKey, allMilestones, TimeSpan.FromMinutes(10));
            }

            var filtered = allMilestones.AsEnumerable();

            MilestoneStatus? parsedStatus = null;
            if (!string.IsNullOrWhiteSpace(query.Status) &&
                Enum.TryParse<MilestoneStatus>(query.Status, true, out var statusValue))
            {
                parsedStatus = statusValue;
            }

            if ((query.From.HasValue || query.To.HasValue) &&
                parsedStatus != MilestoneStatus.Completed)
            {
                throw new Exception("Date filters (from/to) can only be used when status is Completed.");
            }

            if (query.RedmineProjectId.HasValue)
                filtered = filtered.Where(m => m.RedmineProjectId == query.RedmineProjectId.Value);

            if (parsedStatus.HasValue)
                filtered = filtered.Where(m => m.Status == parsedStatus.Value);

            if (query.From.HasValue)
                filtered = filtered.Where(m => m.CompletedAt.HasValue && m.CompletedAt.Value >= query.From.Value);

            if (query.To.HasValue)
                filtered = filtered.Where(m => m.CompletedAt.HasValue && m.CompletedAt.Value <= query.To.Value);

            var filteredList = filtered.ToList();
            var totalItems = filteredList.Count;

            var paged = filteredList
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(m => new ProjectMilestoneDto
                {
                    Id = m.Id,
                    RedmineProjectId = m.RedmineProjectId,
                    Name = m.Name,
                    Status = m.Status.ToString(),
                    CompletedAt = m.CompletedAt
                }).ToList();

            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

            return new PagedResponse<ProjectMilestoneDto>
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