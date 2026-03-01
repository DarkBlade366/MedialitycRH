using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Rules;

namespace Domain.Features.Payrolls.Interfaces
{
    public interface IProductivityRuleRepository
    {
        Task<List<ProductivityRule>> GetAllAsync();
        Task<ProductivityRule?> GetByIdAsync(Guid id);
        Task AddAsync(ProductivityRule rule);
        void Update(ProductivityRule rule);
    }
}