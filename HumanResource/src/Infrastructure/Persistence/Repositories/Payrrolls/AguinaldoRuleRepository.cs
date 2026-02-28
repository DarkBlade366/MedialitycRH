using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Payrolls.Interfaces;

namespace Infrastructure.Persistence.Repositories.Payrrolls
{
    public class AguinaldoRuleRepository : IAguinaldoRuleRepository
    {
        private readonly ApiDbContext _context;

        public AguinaldoRuleRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<AguinaldoRule?> GetByIdAsync(Guid id)
        {
            return await _context.AguinaldoRules
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<AguinaldoRule?> GetActiveAsync()
        {
            return await _context.AguinaldoRules
                .FirstOrDefaultAsync(r => r.IsActive);
        }

        public async Task<IReadOnlyCollection<AguinaldoRule>> GetAllAsync()
        {
            return await _context.AguinaldoRules
                .ToListAsync();
        }

        public async Task AddAsync(AguinaldoRule rule)
        {
            await _context.AguinaldoRules.AddAsync(rule);
        }

        public void Update(AguinaldoRule rule)
        {
            _context.AguinaldoRules.Update(rule);
        }

        public void Remove(AguinaldoRule rule)
        {
            _context.AguinaldoRules.Remove(rule);
        }
    }
}
