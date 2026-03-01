using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Employees.Aggregates;
using Domain.Features.Payrolls.Rules;

namespace Domain.Features.Payrolls.Interfaces
{
    public interface IVacationRuleRepository
    {
        Task<List<VacationRule>> GetAllAsync();
        Task<VacationRule?> GetByIdAsync(Guid id);
        Task AddAsync(VacationRule rule);
        void Update(VacationRule rule);
    }
}