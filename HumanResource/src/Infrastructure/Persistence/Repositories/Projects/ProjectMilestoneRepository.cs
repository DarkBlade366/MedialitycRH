using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Projects.Aggregates;
using Domain.Features.Projects.Enums;
using Domain.Features.Projects.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Projects
{
    public class ProjectMilestoneRepository : IProjectMilestoneRepository
    {
        private readonly ApiDbContext _context;
    
        public ProjectMilestoneRepository(ApiDbContext context)
        {
            _context = context;
        }
    
        public async Task<List<ProjectMilestone>> GetByProjectIdAsync(int redmineProjectId)
        {
            return await _context.ProjectMilestones
                .Where(x => x.RedmineProjectId == redmineProjectId)
                .ToListAsync();
        }
    
        public async Task<ProjectMilestone?> GetByProjectAndNameAsync(int projectId, string name)
        {
            return await _context.ProjectMilestones
                .FirstOrDefaultAsync(x =>
                    x.RedmineProjectId == projectId &&
                    x.Name == name);
        }
    
        public async Task AddRangeAsync(List<ProjectMilestone> milestones)
        {
            await _context.ProjectMilestones.AddRangeAsync(milestones);
        }
    
        public async Task<List<ProjectMilestone>> GetCompletedAsync()
        {
            return await _context.ProjectMilestones
                .Where(x => x.Status == MilestoneStatus.Completed)
                .ToListAsync();
        }

        public async Task<List<ProjectMilestone>> GetAllAsync()
        {
            return await _context.ProjectMilestones.ToListAsync();
        }

        public async Task<bool> ExistsAsync(int redmineProjectId, string milestoneName)
        {
            return await _context.ProjectMilestones
                .AnyAsync(m => m.RedmineProjectId == redmineProjectId && 
                                m.Name == milestoneName);
        }

        public void Update(ProjectMilestone milestone)
        {
            _context.ProjectMilestones.Update(milestone);
        }
    }
}