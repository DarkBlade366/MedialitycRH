using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Payrolls.Interfaces;

namespace Infrastructure.Persistence.Repositories.Payrrolls
{
    public class BaseSalaryRuleRepository : IBaseSalaryRuleRepository
    {
        private readonly ApiDbContext _context;

        public BaseSalaryRuleRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<BaseSalaryRule>> GetAllAsync()
        {
            return await _context.BaseSalaryRules
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<BaseSalaryRule?> GetByIdAsync(Guid id)
        {
            return await _context.BaseSalaryRules.FindAsync(id);
        }

        public async Task AddAsync(BaseSalaryRule rule)
        {
            await _context.BaseSalaryRules.AddAsync(rule);
        }

        public void Update(BaseSalaryRule rule)
        {
            _context.BaseSalaryRules.Update(rule);
        }
    }
}
