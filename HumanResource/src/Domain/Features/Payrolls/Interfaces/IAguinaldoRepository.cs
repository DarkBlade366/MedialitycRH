using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Rules;

namespace Domain.Features.Payrolls.Interfaces
{
    public interface IAguinaldoRuleRepository
    {
        Task<AguinaldoRule?> GetByIdAsync(Guid id);
        Task<AguinaldoRule?> GetActiveAsync();
        Task<IReadOnlyCollection<AguinaldoRule>> GetAllAsync();
        Task AddAsync(AguinaldoRule rule);
        void Update(AguinaldoRule rule);
        void Remove(AguinaldoRule rule);
    }
}