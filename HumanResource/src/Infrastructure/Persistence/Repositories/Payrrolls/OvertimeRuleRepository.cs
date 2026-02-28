using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Payrolls.Interfaces;

namespace Infrastructure.Persistence.Repositories.Payrrolls
{
    public class OvertimeRuleRepository : IOvertimeRuleRepository
    {
        private readonly ApiDbContext _context;

        public OvertimeRuleRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<OvertimeRule?> GetByIdAsync(Guid id)
        {
            return await _context.OvertimeRules
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IReadOnlyCollection<OvertimeRule>> GetAllAsync()
        {
            return await _context.OvertimeRules.ToListAsync();
        }

        public async Task<IReadOnlyCollection<OvertimeRule>> GetActiveAsync()
        {
            return await _context.OvertimeRules
                .Where(r => r.IsActive)
                .ToListAsync();
        }

        public async Task AddAsync(OvertimeRule rule)
        {
            await _context.OvertimeRules.AddAsync(rule);
        }

        public void Update(OvertimeRule rule)
        {
            _context.OvertimeRules.Update(rule);
        }

        public void Remove(OvertimeRule rule)
        {
            _context.OvertimeRules.Remove(rule);
        }
    }
}
