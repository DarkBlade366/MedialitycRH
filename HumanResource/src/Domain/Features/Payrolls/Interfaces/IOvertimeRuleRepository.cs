using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Rules;

namespace Domain.Features.Payrolls.Interfaces
{
    public interface IOvertimeRuleRepository
    {
        Task<List<OvertimeRule>> GetAllAsync();
        Task<OvertimeRule?> GetByIdAsync(Guid id);
        Task AddAsync(OvertimeRule rule);
        void Update(OvertimeRule rule);
    }
}