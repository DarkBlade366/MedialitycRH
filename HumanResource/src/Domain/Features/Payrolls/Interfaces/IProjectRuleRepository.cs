using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Rules;

namespace Domain.Features.Payrolls.Interfaces
{
    public interface IProjectRuleRepository
    {
        Task<List<ProjectRule>> GetAllAsync();
        Task<ProjectRule?> GetByIdAsync(Guid id);
        Task AddAsync(ProjectRule rule);
        void Update(ProjectRule rule);
    }
}
