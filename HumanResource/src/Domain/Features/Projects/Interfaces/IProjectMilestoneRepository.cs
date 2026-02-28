using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Projects.Aggregates;

namespace Domain.Features.Projects.Interfaces
{
    public interface IProjectMilestoneRepository
    {
        Task<List<ProjectMilestone>> GetByProjectIdAsync(int redmineProjectId);
        Task<ProjectMilestone?> GetByProjectAndNameAsync(int projectId, string name);
        Task AddRangeAsync(List<ProjectMilestone> milestones);
        Task<List<ProjectMilestone>> GetCompletedAsync();
        Task<List<ProjectMilestone>> GetAllAsync();
    }
}