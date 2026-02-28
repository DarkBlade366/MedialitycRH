using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Payrolls.Interfaces;

namespace Infrastructure.Persistence.Repositories.Payrrolls
{
    public class ProductivityRuleRepository : IProductivityRuleRepository
    {
        private readonly ApiDbContext _context;

        public ProductivityRuleRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<ProductivityRule?> GetByIdAsync(Guid id)
        {
            return await _context.ProductivityRules
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IReadOnlyCollection<ProductivityRule>> GetAllAsync()
        {
            return await _context.ProductivityRules.ToListAsync();
        }

        public async Task<IReadOnlyCollection<ProductivityRule>> GetActiveAsync()
        {
            return await _context.ProductivityRules
                .Where(r => r.IsActive)
                .ToListAsync();
        }

        public async Task AddAsync(ProductivityRule rule)
        {
            await _context.ProductivityRules.AddAsync(rule);
        }

        public void Update(ProductivityRule rule)
        {
            _context.ProductivityRules.Update(rule);
        }

        public void Remove(ProductivityRule rule)
        {
            _context.ProductivityRules.Remove(rule);
        }
    }
}
