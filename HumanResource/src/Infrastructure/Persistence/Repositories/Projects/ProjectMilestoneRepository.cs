using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Projects.Aggregates;
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
    
        public async Task<List<ProjectMilestone>> GetCompletedUnpaidAsync()
        {
            return await _context.ProjectMilestones
                .Where(x => x.CompletedAt != null && !x.IsPaid)
                .ToListAsync();
        }
    }
}