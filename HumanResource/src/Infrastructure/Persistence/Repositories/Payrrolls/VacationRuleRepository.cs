using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Payrolls.Interfaces;

namespace Infrastructure.Persistence.Repositories.Payrrolls
{
    public class VacationRuleRepository : IVacationRuleRepository
    {
        private readonly ApiDbContext _context;

        public VacationRuleRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<VacationRule?> GetByIdAsync(Guid id)
        {
            return await _context.VacationRules
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IReadOnlyCollection<VacationRule>> GetAllAsync()
        {
            return await _context.VacationRules.ToListAsync();
        }

        public async Task<IReadOnlyCollection<VacationRule>> GetActiveAsync()
        {
            return await _context.VacationRules
                .Where(r => r.IsActive)
                .ToListAsync();
        }

        public async Task AddAsync(VacationRule rule)
        {
            await _context.VacationRules.AddAsync(rule);
        }

        public void Update(VacationRule rule)
        {
            _context.VacationRules.Update(rule);
        }

        public void Remove(VacationRule rule)
        {
            _context.VacationRules.Remove(rule);
        }
    }
}
