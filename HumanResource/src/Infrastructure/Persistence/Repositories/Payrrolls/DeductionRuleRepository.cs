using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Payrolls.Interfaces;

namespace Infrastructure.Persistence.Repositories.Payrrolls
{
    public class DeductionRuleRepository : IDeductionRuleRepository
    {
        private readonly ApiDbContext _context;

        public DeductionRuleRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<DeductionRule>> GetAllAsync()
        {
            return await _context.DeductionRules
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<DeductionRule?> GetByIdAsync(Guid id)
        {
            return await _context.DeductionRules.FindAsync(id);
        }

        public async Task AddAsync(DeductionRule rule)
        {
            await _context.DeductionRules.AddAsync(rule);
        }

        public void Update(DeductionRule rule)
        {
            _context.DeductionRules.Update(rule);
        }
    }
}
