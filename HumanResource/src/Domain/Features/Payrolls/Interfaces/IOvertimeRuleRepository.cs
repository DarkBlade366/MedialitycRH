using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Rules;

namespace Domain.Features.Payrolls.Interfaces
{
    public interface IOvertimeRuleRepository
    {
        Task<OvertimeRule?> GetByIdAsync(Guid id);
        Task<IReadOnlyCollection<OvertimeRule>> GetAllAsync();
        Task<IReadOnlyCollection<OvertimeRule>> GetActiveAsync();
        Task AddAsync(OvertimeRule rule);
        void Update(OvertimeRule rule);
        void Remove(OvertimeRule rule);
    }
}