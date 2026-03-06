using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Entities;

namespace Domain.Features.Payrolls.Interfaces
{
    public interface IActivityProductivityWeightRepository
    {
        Task<List<ActivityProductivityWeight>> GetAllAsync();
        Task<ActivityProductivityWeight?> GetByIdAsync(Guid id);
        Task<ActivityProductivityWeight?> GetByRedmineActivityIdAsync(int redmineActivityId);
        Task AddAsync(ActivityProductivityWeight entity);
        void Update(ActivityProductivityWeight entity);
    }
}
