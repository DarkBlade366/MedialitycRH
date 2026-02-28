using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Milestones.DTOs;
using Application.Features.Milestones.Queries;
using Domain.Features.Projects.Enums;
using Domain.Features.Projects.Interfaces;

namespace Application.Features.Milestones.Handlers
{
    public class GetMilestonesPagedHandler
    {
        private readonly IProjectMilestoneRepository _repository;

        public GetMilestonesPagedHandler(IProjectMilestoneRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<ProjectMilestoneDto>> Handle(GetMilestonesPagedQuery query)
        {
            MilestoneStatus? parsedStatus = null;

if (!string.IsNullOrWhiteSpace(query.Status) &&
    Enum.TryParse<MilestoneStatus>(query.Status, true, out var statusValue))
{
    parsedStatus = statusValue;
}

// Validar fechas solo si hay From/To
if ((query.From.HasValue || query.To.HasValue) &&
    parsedStatus != MilestoneStatus.Completed)
{
    throw new Exception("Date filters (from/to) can only be used when status is Completed.");
}

// Ahora sí filtrar
var all = await _repository.GetAllAsync();

if (query.RedmineProjectId.HasValue)
    all = all.Where(m => m.RedmineProjectId == query.RedmineProjectId.Value).ToList();

if (parsedStatus.HasValue)
    all = all.Where(m => m.Status == parsedStatus.Value).ToList();

if (query.From.HasValue)
    all = all.Where(m => m.CompletedAt.HasValue && m.CompletedAt.Value >= query.From.Value).ToList();

if (query.To.HasValue)
    all = all.Where(m => m.CompletedAt.HasValue && m.CompletedAt.Value <= query.To.Value).ToList();
    
            var totalItems = all.Count;

            var paged = all
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