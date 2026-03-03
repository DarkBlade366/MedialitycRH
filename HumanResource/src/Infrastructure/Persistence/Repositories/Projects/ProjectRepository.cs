using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Projects.Aggregates;
using Domain.Features.Projects.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Projects
{
    public class ProjectRepository : IProjectRepository  
    {
        private readonly ApiDbContext _context;

        public ProjectRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<Project?> GetByRedmineIdAsync(int redmineProjectId)
        {
            return await _context.Projects
                .FirstOrDefaultAsync(x => x.RedmineProjectId == redmineProjectId);
        }

        public async Task<List<Project>> GetAllAsync()
        {
            return await _context.Projects.ToListAsync();
        }

        public async Task AddAsync(Project project)
        {
            await _context.Projects.AddAsync(project);
        }

        public void Update(Project project)
        {
            _context.Projects.Update(project);
        }

        public void Delete(Project project)
        {
            _context.Projects.Remove(project);
        }

        public async Task<bool> ExistsAsync(int redmineProjectId)
        {
            return await _context.Projects
                .AnyAsync(p => p.RedmineProjectId == redmineProjectId);
        }
    }
}
