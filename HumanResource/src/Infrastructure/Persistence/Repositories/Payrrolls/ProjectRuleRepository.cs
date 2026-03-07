using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Payrrolls
{
    public class ProjectRuleRepository : IProjectRuleRepository
    {
        private readonly ApiDbContext _context;

        public ProjectRuleRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectRule>> GetAllAsync()
        {
            return await _context.ProjectRules
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ProjectRule?> GetByIdAsync(Guid id)
        {
            return await _context.ProjectRules
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(ProjectRule rule)
        {
            await _context.ProjectRules.AddAsync(rule);
        }

        public void Update(ProjectRule rule)
        {
            _context.ProjectRules.Update(rule);
        }
    }
}
