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

        public async Task<List<OvertimeRule>> GetAllAsync()
        {
            return await _context.OvertimeRules
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<OvertimeRule?> GetByIdAsync(Guid id)
        {
            return await _context.OvertimeRules.FindAsync(id);
        }

        public async Task AddAsync(OvertimeRule rule)
        {
            await _context.OvertimeRules.AddAsync(rule);
        }

        public void Update(OvertimeRule rule)
        {
            _context.OvertimeRules.Update(rule);
        }
    }
}
