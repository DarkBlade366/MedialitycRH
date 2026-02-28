using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Payrolls.Interfaces;

namespace Infrastructure.Persistence.Repositories.Payrrolls
{
    public class MilestoneRuleRepository : IMilestoneRuleRepository
    {
        private readonly ApiDbContext _context;

        public MilestoneRuleRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<MilestoneRule?> GetByIdAsync(Guid id)
        {
            return await _context.MilestoneRules
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IReadOnlyCollection<MilestoneRule>> GetAllAsync()
        {
            return await _context.MilestoneRules.ToListAsync();
        }

        public async Task<IReadOnlyCollection<MilestoneRule>> GetActiveAsync()
        {
            return await _context.MilestoneRules
                .Where(r => r.IsActive)
                .ToListAsync();
        }

        public async Task AddAsync(MilestoneRule rule)
        {
            await _context.MilestoneRules.AddAsync(rule);
        }

        public void Update(MilestoneRule rule)
        {
            _context.MilestoneRules.Update(rule);
        }

        public void Remove(MilestoneRule rule)
        {
            _context.MilestoneRules.Remove(rule);
        }
    }
}
