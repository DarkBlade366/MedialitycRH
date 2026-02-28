using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Payrolls.ValueObjects;

namespace Domain.Features.Payrolls.Interfaces
{
    public interface IMilestoneRuleRepository
    {
        Task<MilestoneRule?> GetByIdAsync(Guid id);
        Task<IReadOnlyCollection<MilestoneRule>> GetAllAsync();
        Task<IReadOnlyCollection<MilestoneRule>> GetActiveAsync();
        Task AddAsync(MilestoneRule rule);
        void Update(MilestoneRule rule);
        void Remove(MilestoneRule rule);
    }
}