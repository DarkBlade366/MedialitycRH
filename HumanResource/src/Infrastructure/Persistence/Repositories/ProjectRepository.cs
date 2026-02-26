using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
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

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}