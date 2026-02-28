using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Rules;

namespace Domain.Features.Payrolls.Interfaces
{
    public interface IProductivityRuleRepository
    {
        Task<ProductivityRule?> GetByIdAsync(Guid id);
        Task<IReadOnlyCollection<ProductivityRule>> GetAllAsync();
        Task<IReadOnlyCollection<ProductivityRule>> GetActiveAsync();
        Task AddAsync(ProductivityRule rule);
        void Update(ProductivityRule rule);
        void Remove(ProductivityRule rule);
    }
}