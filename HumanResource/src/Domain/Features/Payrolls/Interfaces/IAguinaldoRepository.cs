using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Rules;

namespace Domain.Features.Payrolls.Interfaces
{
    public interface IAguinaldoRuleRepository
    {
        Task<List<AguinaldoRule>> GetAllAsync();
        Task<AguinaldoRule?> GetByIdAsync(Guid id);
        Task AddAsync(AguinaldoRule rule);
        void Update(AguinaldoRule rule);
    }
}