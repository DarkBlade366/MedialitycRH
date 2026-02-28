using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Rules;

namespace Domain.Features.Payrolls.Interfaces
{
    public interface IDeductionRuleRepository
    {
        Task<DeductionRule?> GetByIdAsync(Guid id);
        Task<IReadOnlyCollection<DeductionRule>> GetAllAsync();
        Task<IReadOnlyCollection<DeductionRule>> GetActiveAsync();
        Task AddAsync(DeductionRule rule);
        void Update(DeductionRule rule);
        void Remove(DeductionRule rule);
    }
}