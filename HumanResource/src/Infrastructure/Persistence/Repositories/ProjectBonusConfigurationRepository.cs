using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ProjectBonusConfigurationRepository : IProjectBonusConfigurationRepository
    {
        private readonly ApiDbContext _context;

        public ProjectBonusConfigurationRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectBonusConfiguration>> GetAllAsync()
        {
            return await _context.ProjectBonusConfigurations
                .ToListAsync();
        }
    }
}