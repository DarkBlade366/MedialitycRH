using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Rules;

namespace Domain.Features.Payrolls.Interfaces
{
    public interface IBaseSalaryRuleRepository
    {
        Task<List<BaseSalaryRule>> GetAllAsync();
        Task<BaseSalaryRule?> GetByIdAsync(Guid id);
        Task AddAsync(BaseSalaryRule rule);
        void Update(BaseSalaryRule rule);
    }
}