using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Rules;

namespace Domain.Features.Payrolls.Interfaces
{
    public interface IMilestoneRuleRepository
    {
        Task<List<MilestoneRule>> GetAllAsync();
        Task<MilestoneRule?> GetByIdAsync(Guid id);
        Task AddAsync(MilestoneRule rule);
        void Update(MilestoneRule rule);
    }
}