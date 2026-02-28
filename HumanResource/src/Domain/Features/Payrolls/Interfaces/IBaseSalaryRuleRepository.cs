using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Rules;

namespace Domain.Features.Payrolls.Interfaces
{
    public interface IBaseSalaryRuleRepository
    {
        Task<BaseSalaryRule?> GetByIdAsync(Guid id);
        Task<BaseSalaryRule?> GetActiveAsync();
        Task<IReadOnlyCollection<BaseSalaryRule>> GetAllAsync();
        Task AddAsync(BaseSalaryRule rule);
        void Update(BaseSalaryRule rule);
        void Remove(BaseSalaryRule rule);
    }
}