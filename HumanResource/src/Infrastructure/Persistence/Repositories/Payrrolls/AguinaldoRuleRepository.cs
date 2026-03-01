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

        public async Task<List<AguinaldoRule>> GetAllAsync()
        {
            return await _context.AguinaldoRules
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<AguinaldoRule?> GetByIdAsync(Guid id)
        {
            return await _context.AguinaldoRules.FindAsync(id);
        }

        public async Task AddAsync(AguinaldoRule rule)
        {
            await _context.AguinaldoRules.AddAsync(rule);
        }

        public void Update(AguinaldoRule rule)
        {
            _context.AguinaldoRules.Update(rule);
        }
    }
}
