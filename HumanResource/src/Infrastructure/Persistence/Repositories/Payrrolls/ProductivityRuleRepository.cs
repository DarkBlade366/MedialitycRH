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

        public async Task<List<ProductivityRule>> GetAllAsync()
        {
            return await _context.ProductivityRules
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ProductivityRule?> GetByIdAsync(Guid id)
        {
            return await _context.ProductivityRules.FindAsync(id);
        }

        public async Task AddAsync(ProductivityRule rule)
        {
            await _context.ProductivityRules.AddAsync(rule);
        }

        public void Update(ProductivityRule rule)
        {
            _context.ProductivityRules.Update(rule);
        }
    }
}
