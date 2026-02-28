using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Milestones.DTOs;
using Application.Features.Milestones.Queries;
using Domain.Features.Projects.Interfaces;

namespace Application.Features.Milestones.Handlers
{
    public class GetMilestoneByIdHandler
    {
        private readonly IProjectMilestoneRepository _repository;

        public GetMilestoneByIdHandler(IProjectMilestoneRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProjectMilestoneDto?> Handle(GetMilestoneByIdQuery query)
        {
            var milestone = await _repository.GetAllAsync();

            var item = milestone.FirstOrDefault(m => m.Id == query.Id);
            if (item == null) return null;

            return new ProjectMilestoneDto
            {
                Id = item.Id,
                RedmineProjectId = item.RedmineProjectId,
                Name = item.Name,
                Status = item.Status.ToString(),
                CompletedAt = item.CompletedAt
            };
        }
    }
}